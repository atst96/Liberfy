using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class OAuthApi : TokenApiBase
    {
        internal OAuthApi(Tokens tokens) : base(tokens)
        {
            this._oauthBaseUrl = tokens.HostUrl.AbsoluteUri + "oauth/";
        }

        private readonly string _oauthBaseUrl;

        public Task<AccessTokenResponse> GetAccessToken(string code, string redirectUrl = "urn:ietf:wg:oauth:2.0:oob")
        {
            var url = _oauthBaseUrl + "token";

            var query = new Query
            {
                ["client_id"] = this.Tokens.ClientId,
                ["client_secret"] = this.Tokens.ClientSecret,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = redirectUrl,
                ["code"] = code,
            };

            return this.Tokens.SendRequest<AccessTokenResponse>(WebUtility.CreateWebRequest(url, query, "POST"));
        }

        public string GetAuthorizeUrl(string[] scopes, string redirectUri = "urn:ietf:wg:oauth:2.0:oob")
        {
            var url = _oauthBaseUrl + "authorize";

            var query = new Query
            {
                ["scope"] = string.Join(" ", scopes),
                ["response_type"] = "code",
                ["redirect_uri"] = redirectUri,
                ["client_id"] = this.Tokens.ClientId,
            };

            return $"{ url }?{ string.Join("&", query.GetRequestParameters()) }";
        }
    }
}
