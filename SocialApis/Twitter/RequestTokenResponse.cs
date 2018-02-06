using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public class RequestTokenResponse
    {
        private string _oauthToken;
        private string _oauthTokenSecret;
        private bool _oauthCallbackConfirmed;

        public string OAuthToken => _oauthToken;
        public string OAuthTokenSecret => _oauthTokenSecret;
        public bool OAuthCallbackConfirmed => _oauthCallbackConfirmed;

        public string GetAuthorizeUrl()
        {
            return $"https://api.twitter.com/oauth/authorize?oauth_token={ this._oauthToken }";
        }

        public string GetAuthenticateUrl()
        {
            return $"https://api.twitter.com/oauth/authenticate?oauth_token={ this._oauthToken }";
        }

        internal RequestTokenResponse(string value)
        {
            var splitCharacters = new[] { '=' };

            foreach (var pair in value.Split('&'))
            {
                var t = pair.Split(splitCharacters, 2);

                switch (t[0])
                {
                    case "oauth_token":
                        this._oauthToken = t[1]; break;

                    case "oauth_token_secret":
                        this._oauthTokenSecret = t[1]; break;

                    case "oauth_callback_confirmed":
                        this._oauthCallbackConfirmed = bool.TryParse(t[1], out var b) && b; break;
                }
            }
        }
    }
}
