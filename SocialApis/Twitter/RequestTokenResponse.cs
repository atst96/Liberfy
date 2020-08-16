using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public class RequestTokenResponse
    {
        public string OAuthToken { get; }

        public string OAuthTokenSecret { get; }

        public bool IsOAuthCallbackConfirmed { get; }

        public string GetAuthorizeUrl()
        {
            return $"https://api.twitter.com/oauth/authorize?oauth_token={ this.OAuthToken }";
        }

        public string GetAuthenticateUrl()
        {
            return $"https://api.twitter.com/oauth/authenticate?oauth_token={ this.OAuthToken }";
        }

        internal RequestTokenResponse(string value)
        {
            var values = WebUtility.ParseQueryString(value);

            if (values.TryGetValue("oauth_token", out var oauthToken))
            {
                this.OAuthToken = oauthToken;
            }

            if (values.TryGetValue("oauth_token_secret", out var oauthTokenSecret))
            {
                this.OAuthTokenSecret = oauthTokenSecret;
            }

            if (values.TryGetValue("oauth_callback_confirmed", out var oauthCallbackConfirmed))
            {
                if (bool.TryParse(oauthCallbackConfirmed, out bool isConfirmed))
                {
                    this.IsOAuthCallbackConfirmed = isConfirmed;
                }
            }
        }
    }
}
