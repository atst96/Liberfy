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
        public static IAccount FromSetting(AccountSettingBase item)
        {
            switch (item)
            {
                case TwitterAccountItem twitterItem:
                    return new TwitterAccount(twitterItem);

                case MastodonAccountItem mastodonItem:
                    return new MastodonAccount(mastodonItem);

                default:
                    throw new NotImplementedException();
            }
        }
    }

    internal abstract class AccountBase<TApi, TTimeline, TUser, TStatus>
        : NotificationObject, IAccount, IEquatable<IUserInfo>
            where TApi : IApi
            where TTimeline : TimelineBase
    {
        public string ItemId { get; }

        public abstract long Id { get; protected set; }

        public string HostName { get; }

        public abstract ServiceType Service { get; }

        public abstract IApiGateway ApiGateway { get; }

        public abstract IServiceConfiguration ServiceConfiguration { get; }

        public TApi Api { get; protected set; }

        public abstract DataStoreBase<TUser, TStatus> DataStore { get; }

        protected static Setting Setting { get; } = App.Setting;

        protected object LockSharedObject = new object();

        public IUserInfo Info { get; protected set; }

        public AccountCommandGroup Commands { get; }

        public TTimeline Timeline { get; }

        TimelineBase IAccount.Timeline => this.Timeline;

        private AccountBase(long id, Uri hostUrl)
        {
            this.Id = id;
            this.Timeline = this.CreateTimeline();
            this.HostName = hostUrl?.Host;
            this.Commands = new AccountCommandGroup(this);
        }

        protected AccountBase(long userId, Uri hostUrl, TApi api, AccountSettingBase item)
            : this(userId, hostUrl)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.ItemId = item.ItemId;
            this.Api = api;
            this.Info = this.DataStore.GetAccount(item);
        }

        protected AccountBase(long userId, Uri hostUrl, TApi api, TUser account)
            : this(userId, hostUrl)
        {
            this.ItemId = AccountManager.GenerateUniqueId();
            this.Api = api;
            this.Info = this.GetUserInfo(account);
            this.IsLoggedIn = true;
        }

        protected abstract IUserInfo GetUserInfo(TUser account);

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
            if (Config.Debug.LoadTimeline)
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

        private ConcurrentDictionary<long, StatusActivity> _statusActivities = new ConcurrentDictionary<long, StatusActivity>();

        public StatusActivity GetStatusActivity(long originalStatusId)
        {
            return this._statusActivities.GetOrAdd(originalStatusId, _ => new StatusActivity());
        }

        public abstract IValidator Validator { get; }

        public abstract AccountSettingBase ToSetting();

        public virtual void Unload()
        {
            this.Timeline?.Unload();
            this._statusActivities.Clear();
        }

        public bool Equals(IUserInfo other)
        {
            return this.Id == other.Id && this.Service == other.Service;
        }

        public bool Equals(IAccount other)
        {
            return other != null && this.Id == other.Id && this.Service == other.Service;
        }

        public override bool Equals(object obj)
        {
            return (obj is IUserInfo user && this.Equals(user))
                || (obj is IAccount account && this.Equals(account));
        }

        public override int GetHashCode()
        {
            return $"{ this.Id }.Liberfy.Account.{ this.Service }".GetHashCode();
        }

        void IAccount.SetApiTokens(IApi api)
        {
            this.SetApiTokens((TApi)api);
        }

        public abstract void SetApiTokens(TApi api);
    }
}
