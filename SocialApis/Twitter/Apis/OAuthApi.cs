using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SocialApis.Twitter.Apis
{
    public class OAuthApi : ApiBase
    {
        internal OAuthApi(TwitterApi tokens) : base(tokens) { }

        public async Task<RequestTokenResponse> RequestToken(string callbackUrl = null)
        {
            const string endpoint = "https://api.twitter.com/oauth/request_token";

            var parameters = new Query();

            if (!string.IsNullOrEmpty(callbackUrl))
                parameters[OAuthHelper.OAuthParameters.Callback] = callbackUrl;

            var request = WebUtility.CreateOAuthRequest(HttpMethods.POST, endpoint, this.Api, parameters);
            using var response = await request.GetResponseAsync().ConfigureAwait(false);

            return new RequestTokenResponse(StreamUtility.ReadToEnd(response.GetResponseStream()));
        }

        public Task<TwitterApi> AccessToken(RequestTokenResponse response, string verifier)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            return this.AccessToken(response.OAuthToken, verifier);
        }

        public async Task<TwitterApi> AccessToken(string requestToken, string verifier)
        {
            const string endpoint = "https://api.twitter.com/oauth/access_token";

            if (string.IsNullOrEmpty(requestToken))
                throw new ArgumentNullException(nameof(requestToken));

            if (string.IsNullOrEmpty(verifier))
                throw new ArgumentNullException(nameof(verifier));

            var parameters = new Query()
            {
                [OAuthHelper.OAuthParameters.Token] = requestToken,
                [OAuthHelper.OAuthParameters.Verifier] = verifier,
            };

            var request = WebUtility.CreateOAuthRequest(HttpMethods.POST, endpoint, this.Api, parameters);

            using var response = await request.GetResponseAsync().ConfigureAwait(false);
            using var sr = new StreamReader(response.GetResponseStream(), EncodingUtility.UTF8);
            char[] splitCharacters = { '=' };

            foreach (var pair in sr.ReadToEnd().Split('&'))
            {
                var tokens = pair.Split(splitCharacters, 2);

                switch (tokens[0])
                {
                    case "oauth_token":
                        this.Api.AccessToken = tokens[1];
                        break;

                    case "oauth_token_secret":
                        this.Api.AccessTokenSecret = tokens[1];
                        break;

                    case "user_id":
                        this.Api.UserId = long.TryParse(tokens[1], out var userId) ? userId : -1;
                        break;

                    case "screen_name":
                        this.Api.ScreenName = tokens[1];
                        break;
                }
            }

            return this.Api;
        }
    }
}
