using System;
using System.Net;
using System.Threading.Tasks;
using Liberfy.Services;
using Liberfy.Services.Common;
using Liberfy.Services.Mastodon;
using Liberfy.Settings;
using SocialApis.Mastodon;

namespace Liberfy
{
    internal class MastodonAccount : AccountBase<MastodonApi, MastodonTimeline, Account, Status>
    {
        public override long Id { get; protected set; }

        public override ServiceType Service { get; } = ServiceType.Mastodon;

        public MastodonAccount(AccountItem item)
            : base(item)
        {
        }

        public MastodonAccount(MastodonApi tokens, Account account)
            : base(account.Id, account, tokens)
        {
        }

        public override IAccountCommandGroup Commands { get; } = null;

        private MastodonDataStore _dataStore;
        public override DataStoreBase<Account, Status> DataStore => _dataStore ?? (_dataStore = global::Liberfy.DataStore.Mastodon[this.Tokens.HostUrl]);

        public override IValidator Validator { get; } = new MastodonValidator(int.MaxValue);

        public override IServiceConfiguration ServiceConfiguration { get; } = new MastodonServiceConfiguration();

        private IApiGateway _apiGateway;
        public override IApiGateway ApiGateway => this._apiGateway ?? (this._apiGateway = new MastodonApiGateway(this.Tokens));

        protected override MastodonApi TokensFromApiTokenInfo(ApiTokenInfo tokens)
        {
            return new MastodonApi(new Uri(tokens.Host), tokens.ConsumerKey, tokens.ConsumerSecret, tokens.AccessToken);
        }

        protected override MastodonTimeline CreateTimeline() => new MastodonTimeline(this);

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
                var user = await this.Tokens.Accounts.VerifyCredentials().ConfigureAwait(false);

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

        protected override UserInfo GetUserInfo(Account account)
        {
            return this.DataStore.RegisterAccount(account);
        }
    }
}
