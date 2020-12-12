using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Liberfy.Commands;
using Liberfy.Data.Mastodon;
using Liberfy.Managers;
using Liberfy.Services;
using Liberfy.Services.Common;
using Liberfy.Services.Mastodon;
using Liberfy.Services.Mastodon.Accessors;
using Liberfy.Settings;
using SocialApis.Mastodon;

namespace Liberfy
{
    internal class MastodonAccount : AccountBase, IAccount
    {
        private readonly MastodonAccountSetting _setting;

        public long Id { get; protected set; }

        public override ServiceType Service { get; } = ServiceType.Mastodon;

        public MastodonDataManager DataStore { get; }

        public AccountDetail Info { get; }

        public string InstanceHost { get; }

        public MastodonTimeline Timeline { get; }

        public MastodonApi Api { get; protected set; }

        public MastodonAccount(MastodonAccountSetting item)
            : base(false)
        {
            this._setting = item.Clone();
            this.ItemId = item.ItemId;
            this.Api = item.CreateApi();
            this.InstanceHost = item.InstanceUrl.Host;
            this.DataStore = new MastodonDataManager(item.InstanceUrl);
            this.Info = this.DataStore.GetAccount(item);
            this.Timeline = new MastodonTimeline(this);
            this.Commands = new AccountCommandGroup(this);

            ((INotifyPropertyChanged)this.Info).PropertyChanged += this.OnProfileUpdated;
        }

        public MastodonAccount(string itemId, MastodonApi api, Account account)
            : base(true)
        {
            this._setting = new()
            {
                ItemId = itemId,
                InstanceUrl = api.HostUrl,
                Id = account.Id,
            };
            this.ItemId = itemId;
            this.Api = api;
            this.UpdateApiTokens(api);
            this.InstanceHost = api.HostUrl.Host;
            this.DataStore = new MastodonDataManager(api.HostUrl);
            this.Info = this.DataStore.RegisterAccount(account);
            this.Timeline = new MastodonTimeline(this);
            this.Commands = new AccountCommandGroup(this);
            this.OnProfileUpdated(this.Info, new(null));

            ((INotifyPropertyChanged)this.Info).PropertyChanged += this.OnProfileUpdated;
        }

        //public override IAccountCommandGroup Commands { get; } = null;

        private static readonly IDictionary<Uri, MastodonDataManager> _instanceDataStoreMap = new Dictionary<Uri, MastodonDataManager>();

        public static MastodonDataManager GetDataSotre(Uri hostUrl)
        {
            if (_instanceDataStoreMap.TryGetValue(hostUrl, out var dataStore))
            {
                return dataStore;
            }
            else
            {
                dataStore = new MastodonDataManager(hostUrl);

                _instanceDataStoreMap.Add(hostUrl, dataStore);
                return dataStore;
            }
        }

        public override IValidator Validator { get; } = new MastodonValidator(int.MaxValue);

        public override IServiceConfiguration ServiceConfiguration { get; } = new MastodonServiceConfiguration();

        private IApiGateway _apiGateway;
        [Obsolete]
        public override IApiGateway ApiGateway => this._apiGateway ??= (this._apiGateway = new MastodonApiGateway(this));

        private MastodonStatusAccessor _status;
        private MastodonMediaAccessor _media;

        /// <summary>
        /// トゥート関連のアクセサ
        /// </summary>
        public MastodonStatusAccessor Statuses => this._status ??= new MastodonStatusAccessor(this);

        /// <summary>
        /// メディア関連のアクセサ
        /// </summary>
        public MastodonMediaAccessor Media => this._media ??= new MastodonMediaAccessor(this);

        protected override async ValueTask<bool> VerifyCredentials()
        {
            try
            {
                var user = await this.Api.Accounts.VerifyCredentials().ConfigureAwait(false);

                this.DataStore.RegisterAccount(user);

                return true;
            }
            catch (MastodonException mex)
            {
                if (mex.InnerException is WebException wex)
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

        public void UpdateApiTokens(MastodonApi api)
        {
            this.Api = api;

            var setting = this._setting;
            setting.ClientId = api.ApiToken;
            setting.ClientSecret = api.ApiTokenSecret;
            setting.AccessToken = api.AccessToken;
        }


        private void OnProfileUpdated(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case null:
                case "*":
                case nameof(AccountDetail.Name):
                case nameof(AccountDetail.UserName):
                case nameof(AccountDetail.IsProtected):
                case nameof(AccountDetail.ProfileImageUrl):
                    var setting = this._setting;
                    var account = this.Info;

                    setting.DisplayName = account.Name;
                    setting.UserName = account.UserName;
                    setting.Avatar = account.ProfileImageUrl;
                    setting.IsLocked = account.IsProtected;
                    break;
            }
        }

        private void GetDetails()
        {
        }

        public override IAccountSetting GetSetting() => this._setting.Clone();

        protected readonly ConcurrentDictionary<long, StatusActivity> _statusActivities = new();

        public StatusActivity GetStatusActivity(long originalStatusId)
        {
            return this._statusActivities.GetOrAdd(originalStatusId, _ => new StatusActivity());
        }

        protected override void OnActivityStarted()
        {
            this.GetDetails();

            if (Config.Functions.IsLoadTimelineEnabled)
            {
                this.Timeline.Load();
            }
        }
        protected override void OnActivityStopping()
        {
            this.Timeline.Unload();
            this._statusActivities.Clear();
        }
    }
}
