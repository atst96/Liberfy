using SocialApis.Twitter.Apis;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Twitter
{
    public class Tokens : ITokensBase
    {
        private string _consumerKey;
        private string _consumerSecret;
        private string _accessToken;
        private string _accessTokenSecret;

        public long UserId { get; internal set; }

        public string ScreenName { get; internal set; }

        public string ConsumerKey => _consumerKey ?? string.Empty;
        public string ConsumerSecret => _consumerSecret ?? string.Empty;

        public string AccessToken
        {
            get => _accessToken ?? string.Empty;
            internal set => _accessToken = value;
        }

        public string AccessTokenSecret
        {
            get => _accessTokenSecret ?? string.Empty;
            internal set => _accessTokenSecret = value;
        }

        public string ApiToken => _accessToken ?? string.Empty;
        public string ApiTokenSecret => _accessTokenSecret ?? string.Empty;

        public Tokens(string consumerKey, string consumerSecret, string accessToken = null, string accessTokenSecret = null)
        {
            this._consumerKey = consumerKey;
            this._consumerSecret = consumerSecret;
            this._accessToken = accessToken;
            this._accessTokenSecret = accessTokenSecret;
        }

        private OAuthApi _oauth;
        public OAuthApi OAuth => _oauth ?? (this._oauth = new OAuthApi(this));

        private AccountApi _account;
        public AccountApi Account => _account ?? (this._account = new AccountApi(this));

        private StatusesApi _statuses;
        public StatusesApi Statuses => _statuses ?? (this._statuses = new StatusesApi(this));

        private FavoritesApi _favorites;
        public FavoritesApi Favorites => _favorites ?? (_favorites = new FavoritesApi(this));

        private CollectionsApi _collections;
        public CollectionsApi Collections => _collections ?? (_collections = new CollectionsApi(this));

        private MediaApi _media;
        public MediaApi Media => _media ?? (_media = new MediaApi(this));

        private const string _restApiBaseUrl = "https://api.twitter.com/1.1/";

        internal HttpWebRequest CreateRequester(string endpoint, Query query = null, string method = "GET", bool autoSetting = true)
        {
            return WebUtility.CreateOAuthWebRequest(endpoint, this, query, method, autoSetting);
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
            return WebUtility.CreateOAuthWebRequest($"{_restApiBaseUrl}{path}.json", this, query, method, autoSetting);
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
            var webReq = WebUtility.CreateOAuthWebRequest(endpoint, this, query, method);
            return this.SendRequest<T>(webReq);
        }

        internal async Task<T> SendRequest<T>(HttpWebRequest webReq) where T : class
        {
            try
            {
                using (var webRes = await webReq.GetResponseAsync())
                {
                    var obj = await JsonSerializer.DeserializeAsync<T>(webRes.GetResponseStream(), Utf8Json.Resolvers.StandardResolver.AllowPrivate);

                    if (obj is IRateLimit rObj)
                        rObj.RateLimit.Set(webRes.Headers);

                    return obj;
                }
            }
            catch (WebException wex) when (wex.Response != null)
            {
                var response = wex.Response.GetResponseStream();

                var errors = JsonSerializer.Deserialize<TwitterErrorContainer>(response, Utf8Json.Resolvers.StandardResolver.AllowPrivate);
                throw new TwitterException(wex, errors);
            }
        }

        private Task<T> SendApiRequestAsync<T>(string path, Query query, string method) where T : class
        {
            return SendRequest<T>($"{_restApiBaseUrl}{path}.json", query, method);
        }

        internal Task<T> GetRequestAsync<T>(string endpoint, Query query) where T : class
        {
            return SendRequest<T>(endpoint, query, "GET");
        }

        internal Task<T> GetRequestRestApiAsync<T>(string path, Query query = null) where T : class
        {
            return SendApiRequestAsync<T>(path, query, "GET");
        }

        internal Task<T> PostRequestAsync<T>(string endpoint, Query query = null) where T : class
        {
            return SendRequest<T>(endpoint, query, "POST");
        }

        internal Task<T> PostRequestRestApiAsync<T>(string path, Query query = null) where T : class
        {
            return SendApiRequestAsync<T>(path, query, "POST");
        }
    }
}
