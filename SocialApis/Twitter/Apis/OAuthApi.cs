using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SocialApis.Twitter.Apis
{
    public class OAuthApi : ApiBase
    {
        private readonly Uri _oauthBaseUrl = new Uri("https://api.twitter.com/oauth/");

        internal OAuthApi(TwitterApi tokens) : base(tokens)
        {
        }

        private Uri GetOAuthUrl(string path) => new Uri(this._oauthBaseUrl, path);

        public async Task<RequestTokenResponse> RequestToken(string callbackUrl = null)
        {
            var endpoint = this.GetOAuthUrl("request_token");
            var parameters = new Query();

            if (!string.IsNullOrEmpty(callbackUrl))
            {
                parameters.Add(OAuthHelper.OAuthParameters.Callback, callbackUrl);
            }

            using var request = WebUtility.CreateOAuthRequest(HttpMethod.Post, endpoint, this.Api, parameters);
            var response = await this.Api.SendRequest<string>(request).ConfigureAwait(false);

            return new RequestTokenResponse(response);
        }

        public Task<TwitterApi> AccessToken(RequestTokenResponse response, string verifier)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return this.AccessToken(response.OAuthToken, verifier);
        }

        public async Task<TwitterApi> AccessToken(string requestToken, string verifier)
        {
            if (string.IsNullOrEmpty(requestToken))
            {
                throw new ArgumentNullException(nameof(requestToken));
            }

            if (string.IsNullOrEmpty(verifier))
            {
                throw new ArgumentNullException(nameof(verifier));
            }

            var parameters = new Query
            {
                [OAuthHelper.OAuthParameters.Token] = requestToken,
                [OAuthHelper.OAuthParameters.Verifier] = verifier,
            };

            var endpoint = this.GetOAuthUrl("access_token");
            var request = WebUtility.CreateOAuthRequest(HttpMethod.Post, endpoint, this.Api, parameters);
            var response = await this.Api.SendRequest<string>(request).ConfigureAwait(false);

            var values = WebUtility.ParseQueryString(response);

            if (values.TryGetValue("oauth_token", out var oauthToken))
            {
                this.Api.AccessToken = oauthToken;
            }

            if (values.TryGetValue("oauth_token_secret", out var oauthTokenSecret))
            {
                this.Api.AccessTokenSecret = oauthTokenSecret;
            }

            if (values.TryGetValue("user_id", out var userId))
            {
                this.Api.UserId = long.Parse(userId);
            }

            if (values.TryGetValue("screen_name", out var screenName))
            {
                this.Api.ScreenName = screenName;
            }

            return this.Api;
        }
    }
}
