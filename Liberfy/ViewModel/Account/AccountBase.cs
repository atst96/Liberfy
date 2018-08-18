using Liberfy.Commands;
using Liberfy.Settings;
using SocialApis;
using SocialApis.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal abstract class AccountBase : NotificationObject, IEquatable<AccountBase>, IEquatable<UserInfo>
    {
        public abstract SocialService Service { get; }

        protected abstract DataStore DataStore { get; }

        protected static Setting Setting => App.Setting;

        protected object _lockSharedObject = new object();

        private AccountCommands _commands;
        public AccountCommands Commands => this._commands ?? (this._commands = new AccountCommands(this));

        public abstract long Id { get; protected set; }

        public UserInfo Info { get; protected set; }

        public abstract ITokensBase Tokens { get; }

        public TimelineBase Timeline { get; }

        private bool _automaticallyLogin;
        public bool AutomaticallyLogin
        {
            get => this._automaticallyLogin;
            set => this.SetProperty(ref this._automaticallyLogin, value);
        }

        private bool _automaticallyLoadTimeline;
        public bool AutomaticallyLoadTimeline
        {
            get => this._automaticallyLoadTimeline;
            set => this.SetProperty(ref this._automaticallyLoadTimeline, value);
        }

        public AccountBase(AccountItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            this.Id = item.Id;

            this.Info = this.DataStore.Users.GetOrAdd(
                item.Id,
                _ => new UserInfo(item.Id, item.Name, item.ScreenName, item.IsProtected, item.ProfileImageUrl));

            this.SetTokens(item.Token);

            if (item.MutedIds?.Length > 0)
                this.MutedIds.UnionWith(item.MutedIds);

            this.AutomaticallyLogin = item.AutomaticallyLogin;
            this.AutomaticallyLoadTimeline = item.AutomaticallyLoadTimeline;

            this.Timeline = this.CreateTimeline();
        }

        public AccountBase(ITokensBase tokens, IAccount account)
        {
            if (this.Service != account.Service)
                throw new ArgumentException(nameof(account));

            this.Id = (long)account.Id;
            this.Info = this.DataStore.Users.AddOrUpdate(this.Id,
                (_) => new UserInfo(account),
                (_, info) => info.Update(account));

            this.IsLoggedIn = true;

            this.SetTokens(ApiTokenInfo.FromTokens(tokens));
            this.Timeline = this.CreateTimeline();
        }

        public abstract void SetTokens(ApiTokenInfo tokens);

        protected abstract TimelineBase CreateTimeline();

        private bool _isLoading;
        public bool IsLoading
        {
            get => this._isLoading;
            private set => this.SetProperty(ref this._isLoading, value);
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => this._isLoggedIn;
            private set => this.SetProperty(ref this._isLoggedIn, value);
        }

        public abstract Task Load();

        public async ValueTask<bool> TryLogin()
        {
            if (this._isLoggedIn)
                return true;

            this.IsLoading = true;

            bool loggedIn = await this.Login();
            if (loggedIn)
            {
                this.IsLoggedIn = true;
            }

            this.IsLoading = false;

            return loggedIn;
        }

        protected abstract Task<bool> Login();

        public async Task TryLoadDetails()
        {
            this.IsLoading = true;

            await this.LoadDetails();

            this.IsLoading = false;
        }

        protected abstract Task LoadDetails();

        public virtual void StartTimeline() => this.Timeline.Load();

        private string _errorMessage;
        public string ErrorMessage
        {
            get => this._errorMessage;
            private set => this.SetProperty(ref this._errorMessage, value);
        }

        protected void SetErrorMessage(string name, string message)
        {
            lock (this._lockSharedObject)
            {
                var sb = new StringBuilder(this.ErrorMessage);

                if (sb.Length != 0)
                    sb.AppendLine();

                sb.AppendLine($"{ name }の取得に失敗しました：");
                sb.AppendLine(message);

                this.ErrorMessage = sb.ToString();
                sb = null;
            }
        }

        private SortedDictionary<long, StatusActivity> _statusReactions = new SortedDictionary<long, StatusActivity>();

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

        private HashSet<long> _followingIds;
        public HashSet<long> FollowingIds => this._followersIds ?? (this._followersIds = new HashSet<long>());

        private HashSet<long> _followersIds;
        public HashSet<long> FollowersIds => this._followersIds ?? (this._followersIds = new HashSet<long>());

        private HashSet<long> _blockedIds;
        public HashSet<long> BlockedIds => this._blockedIds ?? (this._blockedIds = new HashSet<long>());

        private HashSet<long> _mutedIds;
        public HashSet<long> MutedIds => this._mutedIds ?? (this._mutedIds = new HashSet<long>());

        private HashSet<long> _incomingIds;
        public HashSet<long> IncomingIds => this._incomingIds ?? (this._incomingIds = new HashSet<long>());

        private HashSet<long> _outgoingIds = new HashSet<long>();
        public HashSet<long> OutgoingIds => this._outgoingIds ?? (this._outgoingIds = new HashSet<long>());

        public AccountItem ToSetting() => new AccountItem
        {
            Service = this.Service,
            Id = this.Id,
            Name = this.Info.Name,
            ScreenName = this.Info.ScreenName,
            IsProtected = this.Info.IsProtected,
            ProfileImageUrl = this.Info.ProfileImageUrl,
            Token = ApiTokenInfo.FromTokens(this.Tokens),
            AutomaticallyLogin = this.AutomaticallyLogin,
            AutomaticallyLoadTimeline = this.AutomaticallyLoadTimeline,
            //Columns = this.Timeline.Columns?.Select(c => c.GetOption()),
            MutedIds = this._mutedIds?.ToArray(),
        };

        public static AccountBase FromSetting(AccountItem item)
        {
            if (item.Service == SocialService.Twitter)
                return new TwitterAccount(item);
            else if (item.Service == SocialService.Mastodon)
                // return new MastodonAccount(item);
                throw new NotImplementedException();
            else
                throw new NotImplementedException();
        }

        public virtual void Unload()
        {
            this.Timeline.Unload();
            this._followingIds?.Clear();
            this._followersIds?.Clear();
            this._blockedIds?.Clear();
            this._mutedIds?.Clear();
            this._incomingIds?.Clear();
            this._outgoingIds?.Clear();
            this._statusReactions.Clear();
        }

        public bool Equals(UserInfo user)
        {
            return this.Id == user.Id && this.Service == user.Service;
        }

        public bool Equals(AccountBase other)
        {
            return this.Id == other.Id && this.Service == other.Service;
        }

        public override bool Equals(object obj)
        {
            return (obj is UserInfo user && this.Equals(user))
                || (obj is AccountBase account && this.Equals(account));
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() + $"Liberfy.Account.{ this.Service.ToString() }.".GetHashCode();
        }

        ~AccountBase()
        {
            // this.Tokens = null;
            this._followingIds = null;
            this._followersIds = null;
            this._blockedIds = null;
            this._mutedIds = null;
            this._incomingIds = null;
            this._outgoingIds = null;
            this._statusReactions = null;
            this._lockSharedObject = null;
        }
    }
}
