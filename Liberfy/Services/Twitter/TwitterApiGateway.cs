using Liberfy.Services.Common;
using Liberfy.Services.Twitter.Accessors;
using SocialApis.Twitter;
using System;
using System.Threading.Tasks;

namespace Liberfy.Services.Twitter
{
    // TODO: Gatewayクラス廃止予定
    internal class TwitterApiGateway : IApiGateway
    {
        private readonly TwitterAccount _account;
        private TwitterApi _api => this._account.Api;

        public TwitterApiGateway(TwitterAccount account)
        {
            this._account = account;
        }

        [Obsolete]
        public Task PostStatus(ServicePostParameters parameters) => this._account.Statuses.Tweet(parameters);

        [Obsolete]
        public Task Favorite(StatusItem item) => this._account.Statuses.Favorite(item);

        [Obsolete]
        public Task Unfavorite(StatusItem item) => this._account.Statuses.Unfavorite(item);

        [Obsolete]
        public Task Retweet(StatusItem item) => this._account.Statuses.Retweet(item);

        [Obsolete]
        public Task Unretweet(StatusItem item) => this._account.Statuses.Unretweet(item);
    }
}
