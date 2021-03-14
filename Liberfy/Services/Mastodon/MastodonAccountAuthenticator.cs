using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Managers;
using SocialApis;
using SocialApis.Mastodon;

namespace Liberfy.Services.Mastodon
{
    internal class MastodonAccountAuthenticator : IAccountAuthenticator
    {
        private static readonly string[] ApiScopes = { "read", "write", "follow" };

        private MastodonApi _api;

        public ServiceType Service { get; } = ServiceType.Mastodon;

        public IApi Api => this._api;

        public string AuthorizeUrl { get; private set; }

        public async Task Authentication(Uri instanceUri, string consumerKey, string consumerSecret)
        {
            string url = instanceUri.ToString();

            if (string.IsNullOrEmpty(consumerKey))
            {
                var clientKey = await ClientKeyManager.GetMastodonKey(instanceUri.Host);
                var cachecClientKey = App.Setting.ClientKeys
                    .Where(key => key.Service == ServiceType.Mastodon)
                    .FirstOrDefault(key => string.Equals(key.Host, instanceUri.ToString(), StringComparison.OrdinalIgnoreCase));

                if (cachecClientKey == null)
                {
                    App.Setting.ClientKeys.Add(new ClientKeyCache(instanceUri, clientKey));

                    consumerKey = clientKey.ClientId;
                    consumerSecret = clientKey.ClientSecret;
                }
                else
                {
                    consumerKey = cachecClientKey.ClientId;
                    consumerSecret = cachecClientKey.ClientSecret;
                }
            }

            this._api = new MastodonApi(instanceUri, consumerKey, consumerSecret);

            this.AuthorizeUrl = this._api.OAuth.GetAuthorizeUrl(ApiScopes);

            App.Open(this.AuthorizeUrl);
        }

        public async Task GetAccessToken(string code)
        {
            Uri host = this._api.HostUrl;
            string consumerKey = this._api.ClientId;
            string consumerSecret = this._api.ClientSecret;

            var res = await this._api.OAuth.GetAccessToken(code).ConfigureAwait(false);

            this._api = new MastodonApi(host, consumerKey, consumerSecret, res.AccessToken);
        }
    }
}
