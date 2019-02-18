using SocialApis.Mastodon.Apis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class MastodonApi : IApi
    {
        private readonly string _hostApiBaseUrl = "";
        public Uri HostUrl { get; }

        public string ClientId { get; }
        public string ClientSecret { get; }
        public string AccessToken { get; }

        string IApi.ConsumerKey => this.ClientId;
        string IApi.ConsumerSecret => this.ClientSecret;

        public string ApiToken { get; private set; }
        public string ApiTokenSecret { get; private set; }

        private readonly WebHeaderCollection _authorizeHeader;

        private MastodonApi(Uri hostUrl)
        {
            this._authorizeHeader = new WebHeaderCollection();

            this.HostUrl = hostUrl;
            this._hostApiBaseUrl = hostUrl.AbsoluteUri + "api/v1/";
        }

        public MastodonApi(Uri hostUrl, string clientId, string clientSecret)
            : this(hostUrl)
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
        }

        public MastodonApi(Uri hostUrl, ClientKeyInfo keys)
            : this(hostUrl, keys.ClientId, keys.ClientSecret)
        {
        }

        public MastodonApi(Uri hostUrl, string clientId, string clientSecret, string accessToken)
            : this(hostUrl, clientId, clientSecret)
        {
            this.AccessToken = accessToken;
            this._authorizeHeader.Add(HttpRequestHeader.Authorization, "Bearer " + accessToken);
        }

        internal string GetApiUrl(string path)
        {
            return this._hostApiBaseUrl + path;
        }

        public OAuthApi _oauth;
        public OAuthApi OAuth => this._oauth ?? (this._oauth = new OAuthApi(this));

        private AccountsApi _accounts;
        public AccountsApi Accounts => this._accounts ?? (this._accounts = new AccountsApi(this));

        private DomainBlocksApi _domainBlocks;
        public DomainBlocksApi DomainBlocks => this._domainBlocks ?? (this._domainBlocks = new DomainBlocksApi(this));

        private FavouritesApi _favourites;
        public FavouritesApi Favourites => this._favourites ?? (this._favourites = new FavouritesApi(this));

        private FollowRequestsApi _followRequests;
        public FollowRequestsApi FollowRequests => this._followRequests ?? (this._followRequests = new FollowRequestsApi(this));

        private FollowsApi _follows;
        public FollowsApi Follows => this._follows ?? (this._follows = new FollowsApi(this));

        private InstancesApi _instances;
        public InstancesApi Instances => this._instances ?? (this._instances = new InstancesApi(this));

        private ListsApi _list;
        public ListsApi List => this._list ?? (this._list = new ListsApi(this));

        private MutesApi _mutes;
        public MutesApi Mutes => this._mutes ?? (this._mutes = new MutesApi(this));

        private NotificationsApi _notifications;
        public NotificationsApi Notifications => this._notifications ?? (this._notifications = new NotificationsApi(this));

        private ReportsApi _report;
        public ReportsApi Report => this._report ?? (this._report = new ReportsApi(this));

        private SearchApi _search;
        public SearchApi Search => this._search ?? (this._search = new SearchApi(this));

        private StatusesApi _statuses;
        public StatusesApi Statuses => this._statuses ?? (this._statuses = new StatusesApi(this));

        private TimelinesApi _timelines;
        public TimelinesApi Timelines => this._timelines ?? (this._timelines = new TimelinesApi(this));

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

                return WebUtility.SendRequest<ClientKeyInfo>(WebUtility.CreateWebRequest(HttpMethods.POST, url, query));
            }
        }

        #region CreateRequestFunctions

        internal HttpWebRequest CreateApiRequest(string method, string endpoint, IQuery query = null)
        {
            return WebUtility.CreateWebRequest(method, endpoint, query, this._authorizeHeader);
        }

        internal HttpWebRequest CreateApiGetRequest(string endpoint, IQuery query = null)
        {
            return this.CreateApiRequest(HttpMethods.GET, endpoint, query);
        }

        internal HttpWebRequest CreateApiPostRequest(string endpoint, IQuery query = null)
        {
            return this.CreateApiRequest(HttpMethods.POST, endpoint, query);
        }

        internal HttpWebRequest CreateApiPutRequest(string endpoint, IQuery query = null)
        {
            return this.CreateApiRequest(HttpMethods.PUT, endpoint, query);
        }

        internal HttpWebRequest CreateApiDeleteRequest(string endpoint, IQuery query = null)
        {
            return this.CreateApiRequest(HttpMethods.DELETE, endpoint, query);
        }

        #endregion

        #region CreateRestApiRquest Functions

        internal HttpWebRequest CreateRestApiRequest(string method, string path, IQuery query = null)
        {
            return this.CreateApiRequest(method, this.GetApiUrl(path), query);
        }

        internal HttpWebRequest CreateRestApiGetRequest(string path, IQuery query = null)
        {
            return this.CreateRestApiRequest(HttpMethods.GET, path, query);
        }

        internal HttpWebRequest CreateRestApiPostRequest(string path, IQuery query = null)
        {
            return this.CreateRestApiRequest(HttpMethods.POST, path, query);
        }

        internal HttpWebRequest CreateRestApiPutRequest(string path, IQuery query = null)
        {
            return this.CreateRestApiRequest(HttpMethods.PUT, path, query);
        }

        internal HttpWebRequest CreateRestApiDeleteRequest(string path, IQuery query = null)
        {
            return this.CreateRestApiRequest(HttpMethods.DELETE, path, query);
        }

        #endregion

        #region SendRequest Functions

        internal async Task<string> SendRequestText(HttpWebRequest request)
        {
            try
            {
                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    return await response.GetResponseStream().ReadToEndAsync().ConfigureAwait(false);
                }
            }
            catch (WebException wex) when (wex.Response != null)
            {
                throw MastodonException.FromWebException(wex);
            }
        }

        internal async Task SendRequest(HttpWebRequest request)
        {
            try
            {
                await request.GetResponseAsync().ConfigureAwait(false);
            }
            catch (WebException wex) when (wex.Response != null)
            {
                throw MastodonException.FromWebException(wex);
            }
        }

        internal async Task<T> SendRequest<T>(HttpWebRequest request) where T : class
        {
            try
            {
                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    var stream = response.GetResponseStream();
                    var obj = await JsonUtility.DeserializeAsync<T>(stream).ConfigureAwait(false);

                    if (obj is IRateLimit rlObj)
                    {
                        rlObj.RateLimit = RateLimit.FromHeaders(response.Headers);
                    }

                    return obj;
                }
            }
            catch (WebException wex) when (wex.Response != null)
            {
                throw MastodonException.FromWebException(wex);
            }
        }

        private Task<T> SendApiRequest<T>(string method, string endpoint, IQuery query = null) where T : class
        {
            return this.SendRequest<T>(this.CreateApiRequest(method, endpoint, query));
        }

        private Task SendApiRequest(string method, string endpoint, IQuery query = null)
        {
            return this.SendRequest(this.CreateApiRequest(method, endpoint, query));
        }

        #endregion

        #region API Methods<T>

        internal Task<T> ApiGetRequestAsync<T>(string endpoint, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethods.GET, endpoint, query);
        }

        internal Task<T> ApiPostRquestAsync<T>(string endpoint, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethods.POST, endpoint, query);
        }

        internal Task<T> ApiPutRqeustAsync<T>(string endpoint, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethods.PUT, endpoint, query);
        }

        internal Task<T> ApiDeleteRequestAsync<T>(string endpoint, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethods.DELETE, endpoint, query);
        }

        #endregion

        #region REST-API Methods Task<T>

        internal Task<T> RestApiGetRequestAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethods.GET, this.GetApiUrl(path), query);
        }

        internal Task<T> RestApiPostRequestAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethods.POST, this.GetApiUrl(path), query);
        }

        internal Task<T> RestApiPutRequestAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethods.PUT, this.GetApiUrl(path), query);
        }

        internal Task<T> RestApiDeleteRequestAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethods.DELETE, this.GetApiUrl(path), query);
        }

        #endregion

        #region REST-API Methods No-generics

        internal Task RestApiGetRequestAsync(string path, IQuery query = null)
        {
            return this.SendApiRequest(HttpMethods.GET, this.GetApiUrl(path), query);
        }

        internal Task RestApiPostRequestAsync(string path, IQuery query = null)
        {
            return this.SendApiRequest(HttpMethods.POST, this.GetApiUrl(path), query);
        }

        internal Task RestApiPutRequestAsync(string path, IQuery query = null)
        {
            return this.SendApiRequest(HttpMethods.PUT, this.GetApiUrl(path), query);
        }

        internal Task RestApiDeleteRequestAsync(string path, IQuery query = null)
        {
            return this.SendApiRequest(HttpMethods.DELETE, this.GetApiUrl(path), query);
        }

        #endregion
    }
}
