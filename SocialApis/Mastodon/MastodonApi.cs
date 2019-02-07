using SocialApis.Mastodon.Apis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

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

                return WebUtility.SendRequest<ClientKeyInfo>(WebUtility.CreateWebRequest(url, query, HttpMethods.POST));
            }
        }

        internal HttpWebRequest CreateApiRequest(string endpoint, IQuery query = null, string method = HttpMethods.GET, bool autoSetting = true)
        {
            return WebUtility.CreateWebRequest(endpoint, query, method, this._authorizeHeader, autoSetting);
        }

        internal HttpWebRequest CreateRequester(string endpoint, IQuery query = null, string method = HttpMethods.GET, bool autoSetting = true)
        {
            return this.CreateApiRequest(endpoint, query, method, autoSetting);
        }

        internal HttpWebRequest CreateGetRequester(string endpoint, IQuery query = null, bool autoSetting = true)
        {
            return this.CreateRequester(endpoint, query, HttpMethods.GET, autoSetting);
        }

        internal HttpWebRequest CreatePostRequester(string endpoint, IQuery query = null, bool autoSetting = true)
        {
            return this.CreateRequester(endpoint, query, HttpMethods.POST, autoSetting);
        }

        internal HttpWebRequest CreateRequesterApi(string path, IQuery query = null, string method = HttpMethods.GET, bool autoSetting = true)
        {
            return this.CreateApiRequest(this.GetApiUrl(path), query, method, autoSetting);
        }

        internal HttpWebRequest CreateGetRequesterApi(string path, IQuery query = null, bool autoSetting = true)
        {
            return this.CreateRequesterApi(path, query, HttpMethods.GET, autoSetting);
        }

        internal HttpWebRequest CreatePostRequesterApi(string path, IQuery query = null, bool autoSetting = true)
        {
            return this.CreateRequesterApi(path, query, HttpMethods.POST, autoSetting);
        }

        private Task<T> SendRequest<T>(string endpoint, IQuery query = null, string method = HttpMethods.GET) where T : class
        {
            var request = this.CreateApiRequest(endpoint, query, method);
            return this.SendRequest<T>(request);
        }

        private Task SendRequest(string endpoint, IQuery query = null, string method = HttpMethods.GET)
        {
            var request = this.CreateApiRequest(endpoint, query, method);
            return this.SendRequestVoid(request);
        }

        internal async Task<string> SendRequestText(HttpWebRequest request)
        {
            try
            {
                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    return await StreamUtility.ReadToEndAsync(response.GetResponseStream()).ConfigureAwait(false);
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

            var errors = JsonUtility.Deserialize<MastodonError>(response);
            return new MastodonException(errors, wex);
        }

        internal async Task SendRequestVoid(HttpWebRequest request)
        {
            try
            {
                await request.GetResponseAsync().ConfigureAwait(false);
            }
            catch (WebException wex) when (wex.Response != null)
            {
                throw GetException(wex);
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
                throw GetException(wex);
            }
        }

        private Task<T> SendApiRequestAsync<T>(string path, IQuery query, string method) where T : class
        {
            return this.SendRequest<T>(this.GetApiUrl(path), query, method);
        }

        private Task SendApiRequestAsync(string path, IQuery query, string method)
        {
            return this.SendRequest(this.GetApiUrl(path), query, method);
        }

        internal Task<T> GetRequestAsync<T>(string endpoint, IQuery query = null) where T : class
        {
            return this.SendRequest<T>(endpoint, query, HttpMethods.GET);
        }

        internal Task<T> GetRequestRestApiAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequestAsync<T>(path, query, HttpMethods.GET);
        }

        internal Task<T> PostRequestAsync<T>(string endpoint, IQuery query = null) where T : class
        {
            return this.SendRequest<T>(endpoint, query, HttpMethods.POST);
        }

        internal Task<T> PostRequestRestApiAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequestAsync<T>(path, query, HttpMethods.POST);
        }

        internal Task GetRequestRestApiAsync(string path, IQuery query = null)
        {
            return this.SendApiRequestAsync(path, query, HttpMethods.GET);
        }

        internal Task PostRequestRestApiAsync(string path, IQuery query = null)
        {
            return this.SendApiRequestAsync(path, query, HttpMethods.POST);
        }
    }
}
