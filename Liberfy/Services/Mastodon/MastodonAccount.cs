using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Liberfy.Factories;
using Liberfy.Services;
using Liberfy.Services.Common;
using Liberfy.Services.Mastodon;
using Liberfy.Services.Mastodon.Accessors;
using Liberfy.Settings;
using SocialApis.Mastodon;

namespace Liberfy
{
    internal class MastodonAccount : AccountBase<MastodonApi, MastodonTimeline, Account, Status>
    {
        public override long Id { get; protected set; }

        public override ServiceType Service { get; } = ServiceType.Mastodon;

        public MastodonDataFactory DataStore { get; }

        public MastodonAccount(MastodonAccountItem item)
            : base(item.Id, item.InstanceUrl, item.CreateApi(), item)
        {
            this.DataStore = new MastodonDataFactory(item.InstanceUrl);
            this.Info = this.DataStore.GetAccount(item);
        }

        public MastodonAccount(MastodonApi tokens, Account account)
            : base(account.Id, tokens.HostUrl, tokens)
        {
            this.DataStore = new MastodonDataFactory(tokens.HostUrl);
            this.Info = this.GetUserInfo(account);
        }

        //public override IAccountCommandGroup Commands { get; } = null;

        private static readonly IDictionary<Uri, MastodonDataFactory> _instanceDataStoreMap = new Dictionary<Uri, MastodonDataFactory>();

        public static MastodonDataFactory GetDataSotre(Uri hostUrl)
        {
            if (_instanceDataStoreMap.TryGetValue(hostUrl, out var dataStore))
            {
                return dataStore;
            }
            else
            {
                dataStore = new MastodonDataFactory(hostUrl);

                _instanceDataStoreMap.Add(hostUrl, dataStore);
                return dataStore;
            }
        }

        public override IValidator Validator { get; } = new MastodonValidator(int.MaxValue);

        public override IServiceConfiguration ServiceConfiguration { get; } = new MastodonServiceConfiguration();

        private IApiGateway _apiGateway;
        [Obsolete]
        public override IApiGateway ApiGateway => this._apiGateway ??= (this._apiGateway = new MastodonApiGateway(this));

        protected override MastodonTimeline CreateTimeline() => new MastodonTimeline(this);

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

        public override async Task Load()
        {
            if (await base.Login().ConfigureAwait(false))
            {
                await this.GetDetails().ConfigureAwait(false);
                this.StartTimeline();
            }
        }

        protected override async Task<bool> VerifyCredentials()
        {
            try
            {
                var user = await this.Api.Accounts.VerifyCredentials().ConfigureAwait(false);

                this.Id = user.Id;

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

        protected override Task GetDetails()
        {
            return Task.CompletedTask;
        }

        protected override IUserInfo GetUserInfo(Account account)
        {
            return this.DataStore.RegisterAccount(account);
        }

        public override void SetApiTokens(MastodonApi api)
        {
            this.Api = api;
        }

        public override AccountSettingBase ToSetting()
        {
            return new MastodonAccountItem
            {
                ItemId = this.ItemId,
                InstanceUrl = this.Api.HostUrl,
                Id = this.Info.Id,
                UserName = this.Info.UserName,
                DisplayName = this.Info.Name,
                Avatar = this.Info.ProfileImageUrl,
                Locked = this.Info.IsProtected,
                ClientId = this.Api.ClientId,
                ClientSecret = this.Api.ClientSecret,
                AccessToken = this.Api.AccessToken,
            };
        }
    }
}
