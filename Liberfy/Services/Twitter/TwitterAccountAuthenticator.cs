using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialApis;
using SocialApis.Twitter;
using Conf = Liberfy.Config.Twitter;

namespace Liberfy.Services.Twitter
{
    internal class TwitterAccountAuthenticator : IAccountAuthenticator
    {
        private TwitterApi _api;
        private RequestTokenResponse _session;

        public ServiceType Service { get; } = ServiceType.Twitter;

        public IApi Api => this._api;

        public string AuthorizeUrl { get; private set; }

        public async Task Authentication(Uri instanceUri, string consumerKey, string consumerSecret)
        {
            this._api = string.IsNullOrEmpty(consumerKey)
                ? new TwitterApi(Conf.ConsumerKey, Conf.ConsumerSecret)
                : new TwitterApi(consumerKey, consumerSecret);

            this._session = await this._api.OAuth.RequestToken().ConfigureAwait(false);

            this.AuthorizeUrl = this._session.GetAuthorizeUrl();

            App.Open(this._session.GetAuthorizeUrl());
        }

        public async Task GetAccessToken(string code)
        {
            this._api = await this._api.OAuth.AccessToken(this._session, code).ConfigureAwait(false);
        }
    }
}
