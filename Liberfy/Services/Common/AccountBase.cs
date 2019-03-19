using Liberfy.Commands;
using Liberfy.Services;
using Liberfy.Services.Common;
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
    internal static class AccountBase
    {
        public static IAccount FromSetting(AccountItem item)
        {
            switch (item.Service)
            {
                case ServiceType.Twitter:
                    return new TwitterAccount(item);

                case ServiceType.Mastodon:
                    return new MastodonAccount(item);

                default:
                    throw new NotImplementedException();
            }
        }
    }

    internal abstract class AccountBase<TTokens, TTimeline, TUser, TStatus>
        : NotificationObject, IAccount, IEquatable<UserInfo>
            where TTokens : IApi
            where TTimeline : TimelineBase
    {
        public abstract long Id { get; protected set; }

        public string HostName { get; }

        public abstract ServiceType Service { get; }

        public TTokens Tokens { get; private set; }

        public abstract IApiGateway ApiGateway { get; }

        public abstract IServiceConfiguration ServiceConfiguration { get; }

        public abstract DataStoreBase<TUser, TStatus> DataStore { get; }

        IApi IAccount.Tokens => this.Tokens;

        protected static Setting Setting { get; } = App.Setting;

        protected object LockSharedObject = new object();

        public void SetClient(ApiTokenInfo tokens)
        {
            this.Tokens = this.TokensFromApiTokenInfo(tokens);
        }

        public abstract IAccountCommandGroup Commands { get; }

        public UserInfo Info { get; protected set; }

        public TTimeline Timeline { get; }

        TimelineBase IAccount.Timeline => this.Timeline;

        private AccountBase(long id, Uri host, ApiTokenInfo tokens)
        {
            this.Id = id;
            this.SetClient(tokens);
            this.Timeline = this.CreateTimeline();
            this.HostName = host?.Host;
        }

        protected AccountBase(Uri hostUrl, AccountItem item)
            : this(item.Id, hostUrl, item.Token)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            this.Info = this.DataStore.Accounts
                .GetOrAdd(item.Id, _ => new UserInfo(hostUrl, item.Id, item.Name, item.ScreenName, item.IsProtected, item.ProfileImageUrl));

            if (item.MutedIds?.Length > 0)
                this.MutedIds.UnionWith(item.MutedIds);
        }

        protected AccountBase(long userId, Uri hostUrl, TUser account, IApi tokens)
            : this(userId, hostUrl, ApiTokenInfo.FromTokens(tokens))
        {
            this.Info = this.GetUserInfo(account);
            this.IsLoggedIn = true;
        }

        protected abstract UserInfo GetUserInfo(TUser account);

        protected abstract TTokens TokensFromApiTokenInfo(ApiTokenInfo tokens);

        protected abstract TTimeline CreateTimeline();

        private bool _isLoading;
        private bool _isLoggedIn;

        public bool IsLoading
        {
            get => this._isLoading;
            private set => this.SetProperty(ref this._isLoading, value);
        }

        public bool IsLoggedIn
        {
            get => this._isLoggedIn;
            private set => this.SetProperty(ref this._isLoggedIn, value);
        }

        public abstract Task Load();

        public async ValueTask<bool> Login()
        {
            if (this._isLoggedIn)
                return true;

            this.IsLoading = true;

            bool loggedIn = await this.VerifyCredentials().ConfigureAwait(false);
            if (loggedIn)
            {
                this.IsLoggedIn = true;
            }

            this.IsLoading = false;

            return loggedIn;
        }

        protected abstract Task<bool> VerifyCredentials();

        public async Task LoadDetails()
        {
            this.IsLoading = true;

            await this.GetDetails().ConfigureAwait(false);

            this.IsLoading = false;
        }

        protected abstract Task GetDetails();

        public virtual void StartTimeline()
        {
            if (App.__DEBUG_LoadTimeline)
            {
#pragma warning disable CS0162
                this.Timeline.Load();
#pragma warning restore CS0162
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get => this._errorMessage;
            private set => this.SetProperty(ref this._errorMessage, value);
        }

        protected void SetErrorMessage(string name, string message)
        {
            lock (this.LockSharedObject)
            {
                var beforeStr = string.IsNullOrEmpty(this.ErrorMessage) ? string.Empty : "\n";

                this.ErrorMessage += $"{ beforeStr }{ name }の取得に失敗しました：\n{ message }";
            }
        }

        private SortedDictionary<long, StatusActivity> _statusActivity = new SortedDictionary<long, StatusActivity>();

        public StatusActivity GetActivity(long usreId)
        {
            StatusActivity activity = default;

            if (!this._statusActivity.TryGetValue(usreId, out activity))
            {
                activity = new StatusActivity();
                this._statusActivity.Add(usreId, activity);
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

        public abstract IValidator Validator { get; }

        public AccountItem ToSetting() => new AccountItem
        {
            Service = this.Service,
            Id = this.Id,
            Name = this.Info.Name,
            ScreenName = this.Info.ScreenName,
            IsProtected = this.Info.IsProtected,
            ProfileImageUrl = this.Info.ProfileImageUrl,
            Token = ApiTokenInfo.FromTokens(this.Tokens),
            //Columns = this.Timeline.Columns?.Select(c => c.GetOption()),
            MutedIds = this._mutedIds?.ToArray(),
        };

        public virtual void Unload()
        {
            this.Timeline?.Unload();
            this._followingIds?.Clear();
            this._followersIds?.Clear();
            this._blockedIds?.Clear();
            this._mutedIds?.Clear();
            this._incomingIds?.Clear();
            this._outgoingIds?.Clear();
            this._statusActivity.Clear();
        }

        public bool Equals(UserInfo other)
        {
            return this.Id == other.Id && this.Service == other.Service;
        }

        public bool Equals(IAccount other)
        {
            return this.Id == other.Id && this.Service == other.Service;
        }

        public override bool Equals(object obj)
        {
            return (obj is UserInfo user    && this.Equals(user))
                || (obj is IAccount account && this.Equals(account));
        }

        public override int GetHashCode()
        {
            return $"{ this.Id }.Liberfy.Account.{ this.Service }".GetHashCode();
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
            this._statusActivity = null;
            this.LockSharedObject = null;
        }
    }
}
