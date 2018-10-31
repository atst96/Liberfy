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
    public class OAuthApi : TokenApiBase
    {
        internal OAuthApi(Tokens tokens) : base(tokens) { }

        public async Task<RequestTokenResponse> RequestToken(string callbackUrl = null)
        {
            const string endpoint = "https://api.twitter.com/oauth/request_token";

            var dic = new Query();

            if (!string.IsNullOrEmpty(callbackUrl))
                dic[OAuthHelper.OAuthParameters.Callback] = callbackUrl;

            var webReq = WebUtility.CreateOAuthRequest(endpoint, this.Tokens, dic, "POST");

            using (var webRes = await webReq.GetResponseAsync())
            using (var sr = new StreamReader(webRes.GetResponseStream()))
            {
                return new RequestTokenResponse(sr.ReadToEnd());
            }
        }

        public Task<Tokens> AccessToken(RequestTokenResponse response, string verifier)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            return this.AccessToken(response.OAuthToken, verifier);
        }

        public async Task<Tokens> AccessToken(string requestToken, string verifier)
        {
            const string endpoint = "https://api.twitter.com/oauth/access_token";

            if (string.IsNullOrEmpty(requestToken))
                throw new ArgumentNullException(nameof(requestToken));

            if (string.IsNullOrEmpty(verifier))
                throw new ArgumentNullException(nameof(verifier));

            var dic = new Query()
            {
                [OAuthHelper.OAuthParameters.Token] = requestToken,
                [OAuthHelper.OAuthParameters.Verifier] = verifier,
            };

            var webReq = WebUtility.CreateOAuthRequest(endpoint, this.Tokens, dic, "POST");

            using (var webRes = await webReq.GetResponseAsync())
            using (var sr = new StreamReader(webRes.GetResponseStream()))
            {
                var splitCharacters = new[] { '=' };

                foreach (var pair in sr.ReadToEnd().Split('&'))
                {
                    var t = pair.Split(splitCharacters, 2);

                    switch (t[0])
                    {
                        case "oauth_token":
                            this.Tokens.AccessToken = t[1];  break;

                        case "oauth_token_secret":
                            this.Tokens.AccessTokenSecret = t[1]; break;

                        case "user_id":
                            this.Tokens.UserId = long.TryParse(t[1], out var b) ? b : -1; break;

                        case "screen_name":
                            this.Tokens.ScreenName = t[1]; break;
                    }
                }

                return Tokens;
            }
        }
    }
}
