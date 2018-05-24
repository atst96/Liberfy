using Liberfy.Commands;
using Liberfy.ViewModel;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Liberfy
{
    [DataContract]
    internal class Account : NotificationObject, IEquatable<Account>, IEquatable<User>
    {
        [IgnoreDataMember]
        private bool _isLoading;


        #region JsonSettings

        #region Account info

        [DataMember(Name = "id")]
        public long Id { get; private set; }

        #endregion

        #region Account settings
        [DataMember(Name = "autologin")]
        private bool _automaticallyLogin = true;
        [IgnoreDataMember]
        public bool AutomaticallyLogin
        {
            get => this._automaticallyLogin;
            set => this.SetProperty(ref this._automaticallyLogin, value);
        }

        [DataMember(Name = "load_timeline")]
        private bool _automaticallyLoadTimeline = true;
        [IgnoreDataMember]
        public bool AutomaticallyLoadTimeline
        {
            get => this._automaticallyLoadTimeline;
            set => this.SetProperty(ref this._automaticallyLoadTimeline, value);
        }

        #endregion

        #endregion

        [IgnoreDataMember]
        public bool IsLoading
        {
            get => this._isLoading;
            set => this.SetProperty(ref this._isLoading, value);
        }

        [IgnoreDataMember]
        public UserInfo Info { get; private set; }

        [DataMember(Name = "tokens")]
        public Tokens Tokens { get; private set; }

        [IgnoreDataMember] private HashSet<long> _following = new HashSet<long>();
        [IgnoreDataMember] private HashSet<long> _follower = new HashSet<long>();
        [IgnoreDataMember] private HashSet<long> _blocking = new HashSet<long>();
        [IgnoreDataMember] private HashSet<long> _muting = new HashSet<long>();
        [IgnoreDataMember] private HashSet<long> _incoming = new HashSet<long>();
        [IgnoreDataMember] private HashSet<long> _outgoing = new HashSet<long>();
        [IgnoreDataMember] private SortedDictionary<long, StatusActivity> _statusReactions = new SortedDictionary<long, StatusActivity>();

        public StatusActivity GetStatusActivity(long user_id)
        {
            StatusActivity activity;

            if (!this._statusReactions.TryGetValue(user_id, out activity))
            {
                activity = new StatusActivity();
                this._statusReactions.Add(user_id, activity);
            }

            return activity;
        }

        [IgnoreDataMember]
        public Timeline Timeline { get; }

        public Account(AccountItem item)
        {
            if (item == null)
                throw new Exception();

            this.Id = item.Id;

            this.Info = DataStore.Users.GetOrAdd(
                item.Id,
                _ => new UserInfo(item.Id, item.Name, item.ScreenName, item.IsProtected, item.ProfileImageUrl));

            this.SetTokens(item.Token.ToCoreTweetTokens());
            this.Timeline = new Timeline(this, item.Columns);
        }

        public Account(Tokens tokens, IEnumerable<ColumnOptionBase> columnOptions = null) : this(tokens, null, columnOptions) { }

        public Account(Tokens tokens, User user, IEnumerable<ColumnOptionBase> columnOptions = null)
        {
            if (user == null)
            {
                this.Id = tokens.UserId;
                this.Info = DataStore.Users.GetOrAdd(
                    Id, _ => new UserInfo(Id, tokens.ScreenName, tokens.ScreenName, false, null));
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

        [IgnoreDataMember]
        private bool _isLoggedIn;
        [IgnoreDataMember]
        public bool IsLoggedIn
        {
            get => this._isLoggedIn;
            set => this.SetProperty(ref _isLoggedIn, value);
        }

        [IgnoreDataMember]
        public bool IsMetadataLoaded { get; set; }

        [IgnoreDataMember]
        private AccountLoginStatus _loginStatus;
        [IgnoreDataMember]
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

        public override bool Equals(object obj)
        {
            return (obj is Account account && this.Equals(account))
                || (obj is User user && this.Equals(user));
        }

        public bool Equals(Account other) => other?.Id == this.Id;

        public bool Equals(User other) => other?.Id == this.Id;

        public override int GetHashCode()
        {
            return Id.GetHashCode() + "Liberfy.Account".GetHashCode();
        }

        public Task LoadAccountDetails()
        {
            return Task.WhenAll(
                this.LoadFollower(),
                this.LoadFollowing(),
                this.LoadBlock(),
                this.LoadMuting(),
                this.LoadOutgoing(),
                this.LoadIncoming()
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

        public object _lockSharedObject = new object();

        #region LoadDetailsメソッド郡

        private Task LoadFollowing() => this.GetIdsList(this.Tokens.Friends.Ids, _follower, "フォロー中一覧");

        private Task LoadFollower() => this.GetIdsList(this.Tokens.Followers.Ids, _follower, "フォロワー一覧");

        private Task LoadBlock() => this.GetIdsList(this.Tokens.Blocks.Ids, _blocking, "ブロック中一覧");

        private Task LoadMuting() => this.GetIdsList(this.Tokens.Mutes.Ids, _muting, "ミュート中一覧");

        private Task LoadIncoming() => this.GetIdsList(this.Tokens.Friendships.Incoming, _incoming, "フォロー申請一覧");

        private Task LoadOutgoing() => this.GetIdsList(this.Tokens.Friendships.Outgoing, _outgoing, "フォロー申請中一覧");

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

        private AccountCommands _commands;
        public AccountCommands Commands => this._commands ?? (this._commands = new AccountCommands(this));

        public void Unload()
        {
            this.Timeline.Unload();
            this.Tokens = null;

            this._following.Clear();
            this._following = null;

            this._follower.Clear();
            this._follower = null;

            this._blocking.Clear();
            this._blocking = null;

            this._muting.Clear();
            this._muting = null;

            this._incoming.Clear();
            this._incoming = null;

            this._outgoing.Clear();
            this._outgoing = null;

            this._statusReactions.Clear();
            this._statusReactions = null;

            this._lockSharedObject = null;
        }

        public AccountItem ToSetting() => new AccountItem
        {
            Id = this.Id,
            Name = this.Info.Name,
            ScreenName = this.Info.ScreenName,
            IsProtected = this.Info.IsProtected,
            ProfileImageUrl = this.Info.ProfileImageUrl,
            Token = ApiTokenInfo.FromCoreTweetTokens(Tokens),
            AutomaticallyLogin = this.AutomaticallyLogin,
            AutomaticallyLoadTimeline = this.AutomaticallyLoadTimeline,
            Columns = this.Timeline.Columns.Select(c => c.GetOption()),
        };
    }

    internal enum AccountLoginStatus
    {
        Success,
        NetworkFaliure,
        NotFound,
    }
}
