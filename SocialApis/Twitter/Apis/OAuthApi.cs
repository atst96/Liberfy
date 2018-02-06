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
        private Tokens _tokens;

        internal OAuthApi(Tokens tokens) : base(tokens) { }

        public async Task<RequestTokenResponse> RequestToken(string callbackUrl = null)
        {
            const string endpoint = "https://api.twitter.com/oauth/request_token";

            var dic = new Query();

            if (!string.IsNullOrEmpty(callbackUrl))
                dic[OAuthHelper.OAuthKeys.Callback] = callbackUrl;

            var webReq = WebUtility.CreateOAuthWebRequest(endpoint, _tokens, dic, "post");

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
                [OAuthHelper.OAuthKeys.Token] = requestToken,
                [OAuthHelper.OAuthKeys.Verifier] = verifier,
            };

            var webReq = WebUtility.CreateOAuthWebRequest(endpoint, _tokens, dic, "post");

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
                            this._tokens.AccessToken = t[1];  break;

                        case "oauth_token_secret":
                            this._tokens.AccessTokenSecret = t[1]; break;

                        case "user_id":
                            this._tokens.UserId = long.TryParse(t[1], out var b) ? b : -1; break;

                        case "screen_name":
                            this._tokens.ScreenName = t[1]; break;
                    }
                }

                return _tokens;
            }
        }
    }
}
