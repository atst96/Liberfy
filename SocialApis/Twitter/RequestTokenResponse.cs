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
            char[] splitCharacters = { '=' };

            foreach (var pair in value.Split('&'))
            {
                var tokens = pair.Split(splitCharacters, 2);

                switch (tokens[0])
                {
                    case "oauth_token":
                        this.OAuthToken = tokens[1];
                        break;

                    case "oauth_token_secret":
                        this.OAuthTokenSecret = tokens[1];
                        break;

                    case "oauth_callback_confirmed":
                        if (bool.TryParse(tokens[1], out bool isConfirmed))
                        {
                            this.IsOAuthCallbackConfirmed = isConfirmed;
                        }
                        break;
                }
            }
        }
    }
}
