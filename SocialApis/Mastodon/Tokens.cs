using SocialApis.Mastodon.Apis;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Mastodon
{
    public class Tokens : ITokensBase
    {
        private string _apiBaseUrl = "";
        public Uri HostUrl { get; private set; }

        public string ClientId { get; }
        public string ClientSecret { get; }
        public string AccessToken { get; }

        string ITokensBase.ConsumerKey => this.ClientId;
        string ITokensBase.ConsumerSecret => this.ClientSecret;

        public string ApiToken { get; private set; }
        public string ApiTokenSecret { get; private set; }

        private WebHeaderCollection _authorizeHeader;

        private Tokens(Uri hostUrl)
        {
            this._authorizeHeader = new WebHeaderCollection();

            this.HostUrl = hostUrl;
            this._apiBaseUrl = hostUrl.AbsoluteUri + "api/v1/";
        }

        public Tokens(Uri hostUrl, string clientId, string clientSecret)
            : this(hostUrl)
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
        }

        public Tokens(Uri hostUrl, ClientKeyInfo keys)
            : this(hostUrl, keys.ClientId, keys.ClientSecret) { }

        public Tokens(Uri hostUrl, string clientId, string clientSecret, string accessToken)
            : this(hostUrl, clientId, clientSecret)
        {
            this.AccessToken = accessToken;
            this._authorizeHeader.Add(HttpRequestHeader.Authorization, $"Bearer { accessToken }");
        }

        internal string GetApiUrl(string path)
        {
            return this._apiBaseUrl + path;
        }

        public OAuthApi _oauth;
        public OAuthApi OAuth => _oauth ?? (_oauth = new OAuthApi(this));

        public AccountsApi _accounts;
        public AccountsApi Accounts => _accounts ?? (_accounts = new AccountsApi(this));

        public static class Apps
        {
            public static Task<ClientKeyInfo> Register(Uri hostUrl, string clientName, string[] scopes, string redirectUris = "urn:ietf:wg:oauth:2.0:oob", Uri website = null)
            {
                var query = new Query
                {
                    ["client_name"] = clientName,
                    ["redirect_uris"] = redirectUris,
                    ["scopes"] = string.Join(" ", scopes),
                };

                if (website != null)
                    query["website"] = website.AbsoluteUri;

                string url = hostUrl.AbsoluteUri + "api/v1/apps";

                return WebUtility.SendRequest<ClientKeyInfo>(WebUtility.CreateWebRequest(url, query, "POST"));
            }
        }

        internal HttpWebRequest CreateApiRequest(string endpoint, Query query = null, string method = "GET", bool autoSetting = true)
        {
           return WebUtility.CreateWebRequest(endpoint, query, method, this._authorizeHeader, autoSetting);
        }

        internal HttpWebRequest CreateRequester(string endpoint, Query query = null, string method = "GET", bool autoSetting = true)
        {
            return this.CreateApiRequest(endpoint, query, method, autoSetting);
        }

        internal HttpWebRequest CreateGetRequester(string endpoint, Query query = null, bool autoSetting = true)
        {
            return this.CreateRequester(endpoint, query, "GET", autoSetting);
        }

        internal HttpWebRequest CreatePostRequester(string endpoint, Query query = null, bool autoSetting = true)
        {
            return this.CreateRequester(endpoint, query, "POST", autoSetting);
        }

        internal HttpWebRequest CreateRequesterApi(string path, Query query = null, string method = "GET", bool autoSetting = true)
        {
            return this.CreateApiRequest($"{ this._apiBaseUrl }{ path }", query, method, autoSetting);
        }

        internal HttpWebRequest CreateGetRequesterApi(string path, Query query = null, bool autoSetting = true)
        {
            return this.CreateRequesterApi(path, query, "GET", autoSetting);
        }

        internal HttpWebRequest CreatePostRequesterApi(string path, Query query = null, bool autoSetting = true)
        {
            return this.CreateRequesterApi(path, query, "POST", autoSetting);
        }

        private Task<T> SendRequest<T>(string endpoint, Query query = null, string method = "GET") where T : class
        {
            var webReq = this.CreateApiRequest(endpoint, query, method);
            return this.SendRequest<T>(webReq);
        }

        internal async Task<string> SendRequestText(HttpWebRequest webReq)
        {
            try
            {
                using (var webRes = await webReq.GetResponseAsync())
                using (var sr = new StreamReader(webRes.GetResponseStream(), Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException wex) when (wex.Response != null)
            {
                throw GetException(wex);
            }
        }

        private static MastodonException GetException(WebException wex)
        {
            var response = wex.Response.GetResponseStream();

            var errors = JsonSerializer.Deserialize<MastodonError>(response, Utf8Json.Resolvers.StandardResolver.AllowPrivate);
            return new MastodonException(errors, wex);
        }

        internal async Task<T> SendRequest<T>(HttpWebRequest webReq) where T : class
        {
            try
            {
                using (var webRes = await webReq.GetResponseAsync())
                {
                    var str = webRes.GetResponseStream();
                    var obj = await JsonSerializer.DeserializeAsync<T>(str, Utf8Json.Resolvers.StandardResolver.AllowPrivate);

                    if (obj is IRateLimit rObj)
                        rObj.RateLimit.Set(webRes.Headers);

                    return obj;
                }
            }
            catch (WebException wex) when (wex.Response != null)
            {
                throw GetException(wex);
            }
        }

        private Task<T> SendApiRequestAsync<T>(string path, Query query, string method) where T : class
        {
            return this.SendRequest<T>($"{ this._apiBaseUrl }{ path }", query, method);
        }

        internal Task<T> GetRequestAsync<T>(string endpoint, Query query) where T : class
        {
            return this.SendRequest<T>(endpoint, query, "GET");
        }

        internal Task<T> GetRequestRestApiAsync<T>(string path, Query query = null) where T : class
        {
            return this.SendApiRequestAsync<T>(path, query, "GET");
        }

        internal Task<T> PostRequestAsync<T>(string endpoint, Query query = null) where T : class
        {
            return this.SendRequest<T>(endpoint, query, "POST");
        }

        internal Task<T> PostRequestRestApiAsync<T>(string path, Query query = null) where T : class
        {
            return this.SendApiRequestAsync<T>(path, query, "POST");
        }
    }
}
