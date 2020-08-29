using SocialApis.Core;
using SocialApis.Mastodon.Apis;
using SocialApis.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class MastodonApi : ApiAccessor, IApi
    {
        private readonly string _hostApiBaseUrl = "";
        public Uri HostUrl { get; }

        public string ClientId { get; }
        public string ClientSecret { get; private set; }
        public string AccessToken { get; private set; }

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

        internal Uri GetApiUrl(string path)
        {
            return new Uri(this._hostApiBaseUrl + path);
        }

        public OAuthApi _oauth;
        public OAuthApi OAuth => this._oauth ??= new OAuthApi(this);

        private AccountsApi _accounts;
        public AccountsApi Accounts => this._accounts ??= new AccountsApi(this);

        private DomainBlocksApi _domainBlocks;
        public DomainBlocksApi DomainBlocks => this._domainBlocks ??= new DomainBlocksApi(this);

        private FavouritesApi _favourites;
        public FavouritesApi Favourites => this._favourites ??= new FavouritesApi(this);

        private FollowRequestsApi _followRequests;
        public FollowRequestsApi FollowRequests => this._followRequests ??= new FollowRequestsApi(this);

        private FollowsApi _follows;
        public FollowsApi Follows => this._follows ??= new FollowsApi(this);

        private InstancesApi _instances;
        public InstancesApi Instances => this._instances ??= new InstancesApi(this);

        private ListsApi _list;
        public ListsApi List => this._list ??= new ListsApi(this);

        private MediaApi _media;
        public MediaApi Media => this._media ??= new MediaApi(this);

        private MutesApi _mutes;
        public MutesApi Mutes => this._mutes ??= new MutesApi(this);

        private NotificationsApi _notifications;
        public NotificationsApi Notifications => this._notifications ??= new NotificationsApi(this);

        private ReportsApi _report;
        public ReportsApi Report => this._report ??= new ReportsApi(this);

        private SearchApi _search;
        public SearchApi Search => this._search ??= new SearchApi(this);

        private StatusesApi _statuses;
        public StatusesApi Statuses => this._statuses ??= new StatusesApi(this);

        private StreamingApi _streaming;
        public StreamingApi Streaming => this._streaming ??= new StreamingApi(this);

        private TimelinesApi _timelines;
        public TimelinesApi Timelines => this._timelines ??= new TimelinesApi(this);

        private AppsApi _apps;
        public AppsApi Apps => this._apps ??= new AppsApi(this);

        public class AppsApi : ApiBase
        {
            internal AppsApi(MastodonApi api)
                : base(api)
            {
            }

            public Task<ClientKeyInfo> Register(Uri hostUrl, string clientName, string[] scopes, string redirectUris = "urn:ietf:wg:oauth:2.0:oob", Uri website = null)
            {
                var query = new Query
                {
                    ["client_name"] = clientName,
                    ["redirect_uris"] = redirectUris,
                    ["scopes"] = string.Join(" ", scopes),
                };

                if (website != null)
                {
                    query["website"] = website.AbsoluteUri;
                }

                var url = new Uri(hostUrl + "/api/v1/apps");

                var request = WebUtility.CreateWebRequest(HttpMethod.Post, url, query);
                return this.Api.SendRequest<ClientKeyInfo>(request);
            }
        }

        #region CreateRequestFunctions

        internal HttpRequestMessage CreateCustomApiRequest(HttpMethod method, string endpoint)
        {
            return WebUtility.CreateWebRequestSimple(method, endpoint, this._authorizeHeader);
        }

        internal HttpRequestMessage CreateApiRequest(HttpMethod method, Uri endpoint, IQuery query = null)
        {
            return WebUtility.CreateWebRequest(method, endpoint, query, this._authorizeHeader);
        }

        internal HttpRequestMessage CreateApiGetRequest(Uri endpoint, IQuery query = null)
        {
            return this.CreateApiRequest(HttpMethod.Get, endpoint, query);
        }

        internal HttpRequestMessage CreateApiPostRequest(Uri endpoint, IQuery query = null)
        {
            return this.CreateApiRequest(HttpMethod.Post, endpoint, query);
        }

        internal HttpRequestMessage CreateApiPutRequest(Uri endpoint, IQuery query = null)
        {
            return this.CreateApiRequest(HttpMethod.Put, endpoint, query);
        }

        internal HttpRequestMessage CreateApiDeleteRequest(Uri endpoint, IQuery query = null)
        {
            return this.CreateApiRequest(HttpMethod.Delete, endpoint, query);
        }

        #endregion

        #region CreateRestApiRquest Functions

        internal HttpRequestMessage CreateCustomRestApiRequest(HttpMethod method, string path)
        {
            return this.CreateCustomApiRequest(method, this.GetApiUrl(path).AbsoluteUri);
        }

        internal HttpRequestMessage CreateRestApiRequest(HttpMethod method, string path, IQuery query = null)
        {
            return this.CreateApiRequest(method, this.GetApiUrl(path), query);
        }

        internal HttpRequestMessage CreateRestApiGetRequest(string path, IQuery query = null)
        {
            return this.CreateRestApiRequest(HttpMethod.Get, path, query);
        }

        internal HttpRequestMessage CreateRestApiPostRequest(string path, IQuery query = null)
        {
            return this.CreateRestApiRequest(HttpMethod.Post, path, query);
        }

        internal HttpRequestMessage CreateRestApiPutRequest(string path, IQuery query = null)
        {
            return this.CreateRestApiRequest(HttpMethod.Put, path, query);
        }

        internal HttpRequestMessage CreateRestApiDeleteRequest(string path, IQuery query = null)
        {
            return this.CreateRestApiRequest(HttpMethod.Delete, path, query);
        }

        #endregion

        protected override T OnWebRequestSucceed<T>(HttpResponseMessage response)
        {
            var obj = base.OnWebRequestSucceed<T>(response);

            if (obj is IRateLimit rateLimitObject)
            {
                rateLimitObject.RateLimit = RateLimit.FromHeaders(response.Headers);
            }

            return obj;
        }

        protected override void OnWebRequestFailed(HttpResponseMessage response)
        {
            throw MastodonException.FromWebException(response);
        }

        #region SendRequest Functions

        private Task<T> SendApiRequest<T>(HttpMethod method, Uri endpoint, IQuery query = null) where T : class
        {
            var request = this.CreateApiRequest(method, endpoint, query);
            return this.SendRequest<T>(request);
        }

        private async Task SendApiRequest(HttpMethod method, Uri endpoint, IQuery query = null)
        {
            using var request = this.CreateApiRequest(method, endpoint, query);
            await this.SendRequest<string>(request).ConfigureAwait(false);
        }

        #endregion

        #region API Methods<T>

        internal Task<T> ApiGetRequestAsync<T>(string path, IQuery query = null)
            where T : class
        {
            return this.SendApiRequest<T>(HttpMethod.Get, this.GetApiUrl(path), query);
        }

        internal Task<T> ApiPostRquestAsync<T>(string path, IQuery query = null)
            where T : class
        {
            return this.SendApiRequest<T>(HttpMethod.Post, this.GetApiUrl(path), query);
        }

        internal Task<T> ApiPutRqeustAsync<T>(string path, IQuery query = null)
            where T : class
        {
            return this.SendApiRequest<T>(HttpMethod.Put, this.GetApiUrl(path), query);
        }

        internal Task<T> ApiDeleteRequestAsync<T>(string path, IQuery query = null)
            where T : class
        {
            return this.SendApiRequest<T>(HttpMethod.Delete, this.GetApiUrl(path), query);
        }

        #endregion

        #region REST-API Methods Task<T>

        internal Task<T> RestApiGetRequestAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethod.Get, this.GetApiUrl(path), query);
        }

        internal Task<T> RestApiPostRequestAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethod.Post, this.GetApiUrl(path), query);
        }

        internal Task<T> RestApiPutRequestAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethod.Put, this.GetApiUrl(path), query);
        }

        internal Task<T> RestApiDeleteRequestAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequest<T>(HttpMethod.Delete, this.GetApiUrl(path), query);
        }

        #endregion

        #region REST-API Methods No-generics

        internal Task RestApiGetRequestAsync(string path, IQuery query = null)
        {
            return this.SendApiRequest(HttpMethod.Get, this.GetApiUrl(path), query);
        }

        internal Task RestApiPostRequestAsync(string path, IQuery query = null)
        {
            return this.SendApiRequest(HttpMethod.Post, this.GetApiUrl(path), query);
        }

        internal Task RestApiPutRequestAsync(string path, IQuery query = null)
        {
            return this.SendApiRequest(HttpMethod.Put, this.GetApiUrl(path), query);
        }

        internal Task RestApiDeleteRequestAsync(string path, IQuery query = null)
        {
            return this.SendApiRequest(HttpMethod.Delete, this.GetApiUrl(path), query);
        }

        #endregion
    }
}
