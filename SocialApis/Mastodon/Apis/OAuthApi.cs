using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class OAuthApi : ApiBase
    {
        private readonly string _oauthBaseUrl;

        internal OAuthApi(MastodonApi tokens) : base(tokens)
        {
            this._oauthBaseUrl = tokens.HostUrl.AbsoluteUri + "oauth/";
        }

        public Task<AccessTokenResponse> GetAccessToken(string code, string redirectUrl = "urn:ietf:wg:oauth:2.0:oob")
        {
            var url = _oauthBaseUrl + "token";

            var parameters = new Query
            {
                ["client_id"] = this.Api.ClientId,
                ["client_secret"] = this.Api.ClientSecret,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = redirectUrl,
                ["code"] = code,
            };

            return this.Api.SendRequest<AccessTokenResponse>(WebUtility.CreateWebRequest(HttpMethods.POST, url, parameters));
        }

        public string GetAuthorizeUrl(string[] scopes, string redirectUri = "urn:ietf:wg:oauth:2.0:oob")
        {
            var url = _oauthBaseUrl + "authorize";

            var query = new Query
            {
                ["scope"] = string.Join(" ", scopes),
                ["response_type"] = "code",
                ["redirect_uri"] = redirectUri,
                ["client_id"] = this.Api.ClientId,
            };

            return $"{ url }?{ string.Join("&", Query.GetRequestParameterStrings(query)) }";
        }
    }
}
