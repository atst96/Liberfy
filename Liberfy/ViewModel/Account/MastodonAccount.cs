using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Settings;
using SocialApis;
using SocialApis.Mastodon;

namespace Liberfy
{
    internal class MastodonAccount : AccountBase, IAccount<Tokens>
    {
        public override long Id { get; protected set; }

        public override SocialService Service { get; } = SocialService.Mastodon;

        protected override DataStore DataStore { get; } = DataStore.Mastodon;

        public Tokens InternalTokens { get; private set; }

        public override ITokensBase Tokens => this.InternalTokens;

        public MastodonAccount(AccountItem item)
            : base(item)
        {
        }

        public MastodonAccount(Tokens tokens, Account account)
            : base(tokens, account)
        {
        }

        public override void SetTokens(ApiTokenInfo tokens)
        {
            this.InternalTokens = new Tokens(
                new Uri(tokens.Host),
                tokens.ConsumerKey,
                tokens.ConsumerSecret,
                tokens.AccessToken);
        }

        protected override TimelineBase CreateTimeline()
        {
            return new MastodonTimeline(this);
        }

        public override async Task Load()
        {
            if (await base.TryLogin())
            {
                await this.LoadDetails();
                this.StartTimeline();
            }
        }

        protected override async Task<bool> Login()
        {
            try
            {
                var user = await this.InternalTokens.Accounts.VerifyCredentials();

                this.Id = user.Id;

                this.Info.Update(user);

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

        protected override Task LoadDetails()
        {
            return Task.CompletedTask;
        }
    }
}
