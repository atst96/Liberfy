using Liberfy.Commands;
using Liberfy.Settings;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Liberfy
{
    internal sealed class Account : NotificationObject, IEquatable<Account>, IEquatable<User>
    {
        private Setting Setting => App.Setting;

        private object _lockSharedObject = new object();

        private AccountCommands _commands;
        public AccountCommands Commands => this._commands ?? (this._commands = new AccountCommands(this));

        public long Id { get; private set; }

        public UserInfo Info { get; private set; }

        public Tokens Tokens { get; private set; }

        public Timeline Timeline { get; }

        private bool _automaticallyLogin = true;
        public bool AutomaticallyLogin
        {
            get => this._automaticallyLogin;
            set => this.SetProperty(ref this._automaticallyLogin, value);
        }

        private bool _automaticallyLoadTimeline = true;
        public bool AutomaticallyLoadTimeline
        {
            get => this._automaticallyLoadTimeline;
            set => this.SetProperty(ref this._automaticallyLoadTimeline, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => this._isLoading;
            set => this.SetProperty(ref this._isLoading, value);
        }

        private HashSet<long> _followingIds = new HashSet<long>();
        private HashSet<long> _followerIds = new HashSet<long>();
        private HashSet<long> _blockedIds = new HashSet<long>();
        private HashSet<long> _mutedIds = new HashSet<long>();
        private HashSet<long> _incomingIds = new HashSet<long>();
        private HashSet<long> _outgoingIds = new HashSet<long>();
        private SortedDictionary<long, StatusActivity> _statusReactions = new SortedDictionary<long, StatusActivity>();

        public Account(AccountItem item)
        {
            if (item == null)
                throw new Exception();

            this.Id = item.Id;

            this.Info = DataStore.Users.GetOrAdd(
                item.Id,
                _ => new UserInfo(item.Id, item.Name, item.ScreenName, item.IsProtected, item.ProfileImageUrl));

            this.SetTokens(item.Token.ToTokens());

            if (item.MutedIds?.Length > 0)
                this._mutedIds.UnionWith(item.MutedIds);

            this.Timeline = new Timeline(this, item.Columns);
        }

        public Account(Tokens tokens, IEnumerable<ColumnOptionBase> columnOptions = null)
            : this(tokens, null, columnOptions)
        {
        }

        public Account(Tokens tokens, User user, IEnumerable<ColumnOptionBase> columnOptions = null)
        {
            if (user == null)
            {
                this.Id = tokens.UserId;
                this.Info = DataStore.Users.GetOrAdd(
                    this.Id,
                    id => new UserInfo(id, tokens.ScreenName, tokens.ScreenName, false, null));
            }
            else
            {
                this.Id = (long)user.Id;
                this.Info = new UserInfo(user);
                this.Info = DataStore.Users.AddOrUpdate(Id,
                    _ => new UserInfo(user),
                    (_, info) => info.Update(user));
            }

            this.SetTokens(tokens);
            this.Timeline = new Timeline(this, columnOptions);
        }

        public void SetTokens(Tokens t)
        {
            this.Tokens = new Tokens(
                t.ConsumerKey,
                t.ConsumerSecret,
                t.AccessToken,
                t.AccessTokenSecret);
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => this._isLoggedIn;
            set => this.SetProperty(ref _isLoggedIn, value);
        }

        private AccountLoginStatus _loginStatus;
        public AccountLoginStatus LoginStatus
        {
            get => this._loginStatus;
            set => this.SetProperty(ref _loginStatus, value);
        }

        public async ValueTask<bool> Login()
        {
            if (this._isLoggedIn) return true;

            try
            {
                var user = await this.Tokens.Account.VerifyCredentials(new SocialApis.Query
                {
                    ["include_entities"] = true,
                    ["skip_status"] = true,
                    ["include_email"] = false
                });

                this.Id = user.Id.Value;

                this.Info.Update(user);

                this.IsLoggedIn = true;
                this.LoginStatus = AccountLoginStatus.Success;

                return true;
            }
            catch (TwitterException tex)
            {
                if (tex.InnerException is WebException wex)
                {
                    switch (wex.Status)
                    {
                        case WebExceptionStatus.Success:
                            break;
                    }
                }
            }

            return false;
        }

        public StatusActivity GetActivity(long user_id)
        {
            StatusActivity activity;

            if (!this._statusReactions.TryGetValue(user_id, out activity))
            {
                activity = new StatusActivity();
                this._statusReactions.Add(user_id, activity);
            }

            return activity;
        }

        public Task LoadAccountDetails()
        {
            return Task.WhenAll(
                this.LoadFollowerIds(),
                this.LoadFollowingIds(),
                this.LoadBlockedIds(),
                this.LoadMutedIds(),
                this.LoadOutgoingIds(),
                this.LoadIncomingIds()
            );
        }

        public void StartTimeline()
        {
            this.Timeline.Load();
        }

        private string _loadError;
        public string LoadErorr
        {
            get => this._loadError;
            private set => this.SetProperty(ref this._loadError, value);
        }

        private void SetLoadError(string name, string message)
        {
            lock (this._lockSharedObject)
            {
                var sb = new StringBuilder(this._loadError);

                if (sb.Length != 0)
                    sb.AppendLine();

                sb.AppendLine($"{ name }の取得に失敗しました：");
                sb.AppendLine(message);

                this.LoadErorr = sb.ToString();
                sb = null;
            }
        }

        #region LoadDetailsメソッド郡

        private Task LoadFollowingIds()
            => this.GetIdsList(this.Tokens.Friends.Ids, _followerIds, "フォロー中一覧");

        private Task LoadFollowerIds()
            => this.GetIdsList(this.Tokens.Followers.Ids, _followerIds, "フォロワー一覧");

        private Task LoadBlockedIds()
        {
            return Setting.GetBlockedIdsAtLoadingAccount
                ? this.GetIdsList(this.Tokens.Blocks.Ids, _blockedIds, "ブロック中一覧")
                : Task.CompletedTask;
        }

        private Task LoadMutedIds()
        {
            return Setting.GetMutedIdsAtLoadingAccount
                ? this.GetIdsList(this.Tokens.Mutes.Ids, _mutedIds, "ミュート中一覧")
                : Task.CompletedTask;
        }

        private Task LoadIncomingIds()
        {
            return this.Info.IsProtected
                ? this.GetIdsList(this.Tokens.Friendships.Incoming, _incomingIds, "フォロー申請一覧")
                : Task.CompletedTask;
        }

        private Task LoadOutgoingIds()
            => this.GetIdsList(this.Tokens.Friendships.Outgoing, _outgoingIds, "フォロー申請中一覧");

        private async Task GetIdsList(Func<int, Task<CursoredIdsResponse>> apiCallFunc, HashSet<long> hashSet, string dataLabel)
        {
            try
            {
                int cursor = -1;

                do
                {
                    var ids = await apiCallFunc(cursor);
                    cursor = ids.NextCursor ?? 0;

                    hashSet.UnionWith(ids.Ids);
                }
                while (cursor != 0);
            }
            catch (Exception ex)
            {
                this.SetLoadError(dataLabel, ex.Message);
            }
        }

        #endregion LoadDetails

        public AccountItem ToSetting() => new AccountItem
        {
            Service = SocialService.Twitter,
            Id = this.Id,
            Name = this.Info.Name,
            ScreenName = this.Info.ScreenName,
            IsProtected = this.Info.IsProtected,
            ProfileImageUrl = this.Info.ProfileImageUrl,
            Token = ApiTokenInfo.FromTokens(Tokens),
            AutomaticallyLogin = this.AutomaticallyLogin,
            AutomaticallyLoadTimeline = this.AutomaticallyLoadTimeline,
            Columns = this.Timeline.Columns.Select(c => c.GetOption()),
            MutedIds = this._mutedIds.ToArray(),
        };

        public void Unload()
        {
            this.Timeline.Unload();
            this._followingIds.Clear();
            this._followerIds.Clear();
            this._blockedIds.Clear();
            this._mutedIds.Clear();
            this._incomingIds.Clear();
            this._outgoingIds.Clear();
            this._statusReactions.Clear();
        }

        ~Account()
        {
            this.Tokens = null;
            this._followingIds = null;
            this._followerIds = null;
            this._blockedIds = null;
            this._mutedIds = null;
            this._incomingIds = null;
            this._outgoingIds = null;
            this._statusReactions = null;
            this._lockSharedObject = null;
        }

        public override bool Equals(object obj)
        {
            return (obj is Account account && this.Equals(account))
                || (obj is User user && this.Equals(user));
        }

        public bool Equals(Account other) => other?.Id == this.Id;

        public bool Equals(User other) => other?.Id == this.Id;

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() + "Liberfy.Account".GetHashCode();
        }
    }

    internal enum AccountLoginStatus
    {
        Success,
        NetworkFaliure,
        NotFound,
    }
}
