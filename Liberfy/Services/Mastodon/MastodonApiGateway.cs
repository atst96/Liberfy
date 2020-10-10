using Liberfy.Services.Common;
using Liberfy.Services.Mastodon.Accessors;
using Liberfy.ViewModels;
using SocialApis;
using SocialApis.Mastodon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Services.Mastodon
{
    // TODO: Gatewayクラス廃止予定
    internal class MastodonApiGateway : IApiGateway
    {
        private readonly MastodonAccount _account;
        private readonly MastodonApi _api;

        public MastodonApiGateway(MastodonAccount account)
        {
            this._account = account;
            this._api = account.Api;
        }

        [Obsolete]
        public Task PostStatus(ServicePostParameters parameters) => this._account.Statuses.Toat(parameters);

        [Obsolete]
        public Task Favorite(StatusItem item) => this._account.Statuses.Favorite(item);

        [Obsolete]
        public Task Unfavorite(StatusItem item) => this._account.Statuses.Unfavorite(item);

        [Obsolete]
        public Task Retweet(StatusItem item) => this._account.Statuses.Reblog(item);

        [Obsolete]
        public Task Unretweet(StatusItem item) => this._account.Statuses.Unreblog(item);
    }
}
