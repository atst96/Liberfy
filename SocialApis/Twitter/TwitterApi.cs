using SocialApis.Twitter.Apis;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Utf8Json;
using System.Collections.Generic;
using SocialApis.Core;
using System.Net.Http;
using System;
using SocialApis.Utils;

namespace SocialApis.Twitter
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class TwitterApi : ApiAccessor, IApi
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private string _accessToken;
        private string _accessTokenSecret;

        /// <summary>
        /// アカウントID
        /// </summary>
        public long UserId { get; internal set; }

        /// <summary>
        /// スクリーンネーム
        /// </summary>
        public string ScreenName { get; internal set; }

        /// <summary>
        /// ConsumerKey
        /// </summary>
        public string ConsumerKey => this._consumerKey ?? string.Empty;

        /// <summary>
        /// ConsumerSecret
        /// </summary>
        public string ConsumerSecret => this._consumerSecret ?? string.Empty;

        /// <summary>
        /// AccessToken
        /// </summary>
        public string AccessToken
        {
            get => this._accessToken ?? string.Empty;
            internal set => this._accessToken = value;
        }

        /// <summary>
        /// AccessTokenSecret
        /// </summary>
        public string AccessTokenSecret
        {
            get => this._accessTokenSecret ?? string.Empty;
            internal set => this._accessTokenSecret = value;
        }

        public string ApiToken => this._accessToken ?? string.Empty;

        public string ApiTokenSecret => this._accessTokenSecret ?? string.Empty;

        public TwitterApi(string consumerKey, string consumerSecret)
            : this(consumerKey, consumerSecret, null, null)
        {
        }

        public TwitterApi(string consumerKey, string consumerSecret, string accessToken = null, string accessTokenSecret = null)
            : base()
        {
            this._consumerKey = consumerKey;
            this._consumerSecret = consumerSecret;
            this._accessToken = accessToken;
            this._accessTokenSecret = accessTokenSecret;
        }

        private OAuthApi _oauth;
        public OAuthApi OAuth => this._oauth ??= new OAuthApi(this);

        private AccountApi _account;
        public AccountApi Account => this._account ??= new AccountApi(this);

        private StatusesApi _statuses;
        public StatusesApi Statuses => this._statuses ??= new StatusesApi(this);

        private DirectMessageApi _directMessageApi;
        public DirectMessageApi DirectMessage => this._directMessageApi ??= new DirectMessageApi(this);

        private FavoritesApi _favorites;
        public FavoritesApi Favorites => this._favorites ??= new FavoritesApi(this);

        private CollectionsApi _collections;
        public CollectionsApi Collections => this._collections ??= new CollectionsApi(this);

        private MediaApi _mediaApi;
        public MediaApi Media => this._mediaApi ??= new MediaApi(this);

        private BlocksApi _blocks;
        public BlocksApi Blocks => this._blocks ??= new BlocksApi(this);

        private MutesApi _mutes;
        public MutesApi Mutes => this._mutes ??= new MutesApi(this);

        private UsersApi _users;
        public UsersApi Users => this._users ??= new UsersApi(this);

        private FollowersApi _followers;
        public FollowersApi Followers => this._followers ??= new FollowersApi(this);

        private FriendsApi _friends;
        public FriendsApi Friends => this._friends ??= new FriendsApi(this);

        private FriendshipsApi _friendships;
        public FriendshipsApi Friendships => this._friendships ??= new FriendshipsApi(this);

        private static readonly Uri RestApiBaseUrl = new Uri("https://api.twitter.com/1.1/");

        private static Uri GetRestApiUrl(string path)
        {
            return new Uri(RestApiBaseUrl, string.Concat(path, ".json"));
        }

        private Task<T> SendRequest<T>(HttpMethod method, Uri endpoint, HttpContent content)
            where T : class
        {
            var request = WebUtility.CreateOAuthRequest(method, endpoint, this, content);

            return this.SendRequest<T>(request);
        }

        private Task<T> SendRequest<T>(HttpMethod method, Uri endpoint, IQuery query = null)
            where T : class
        {
            var request = WebUtility.CreateOAuthRequest(method, endpoint, this, query);

            return this.SendRequest<T>(request);
        }

        protected override T OnWebRequestSucceed<T>(HttpResponseMessage response)
            where T : class
        {
            var responseObj = base.OnWebRequestSucceed<T>(response);

            if (responseObj is IRateLimit rateLimitObject)
            {
                rateLimitObject.RateLimit = RateLimit.FromHeaders(response.Headers);
            }

            return responseObj;
        }

        protected override void OnWebRequestFailed(HttpResponseMessage response)
            => throw TwitterException.FromWebException(response);

        internal async Task<T> SendRequest<T>(HttpWebRequest request) where T : class
        {
            try
            {
                using var response = await request.GetResponseAsync().ConfigureAwait(false);
                var stream = response.GetResponseStream();
                var obj = await JsonUtil.DeserializeAsync<T>(stream).ConfigureAwait(false);

                if (obj is IRateLimit rlObj)
                {
                    rlObj.RateLimit = RateLimit.FromHeaders(response.Headers);
                }

                return obj;
            }
            catch (WebException wex) when (wex.Response != null)
            {
                throw TwitterException.FromWebException(wex);
            }
        }

        internal Task<T> ApiGetRequestAsync<T>(Uri endpoint, IQuery query)
            where T : class
        {
            return this.SendRequest<T>(HttpMethod.Get, endpoint, query);
        }

        internal Task<T> ApiPostRequestAsync<T>(Uri endpoint, IQuery query = null)
            where T : class
        {
            return this.SendRequest<T>(HttpMethod.Post, endpoint, query);
        }

        internal Task<T> ApiGetRequestAsync<T>(Uri endpoint, HttpContent content)
            where T : class
        {
            return this.SendRequest<T>(HttpMethod.Get, endpoint, content);
        }

        internal Task<T> ApiPostRequestAsync<T>(Uri endpoint, HttpContent content = null)
            where T : class
        {
            return this.SendRequest<T>(HttpMethod.Get, endpoint, content);
        }

        internal Task<T> RestApiGetRequestAsync<T>(string path, IQuery query = null)
            where T : class
        {
            var url = GetRestApiUrl(path);
            return this.ApiGetRequestAsync<T>(url, query);
        }

        internal Task<T> RestApiPostRequestAsync<T>(string path, IQuery query = null)
            where T : class
        {
            return this.ApiPostRequestAsync<T>(GetRestApiUrl(path), query);
        }

        //internal Task<T> RestApiGetRequestAsync<T>(string path, HttpContent content = null)
        //    where T : class
        //{
        //    return this.ApiGetRequestAsync<T>(GetRestApiUrl(path), content);
        //}

        //internal Task<T> RestApiPostRequestAsync<T>(string path, HttpContent content = null)
        //    where T : class
        //{
        //    return this.ApiPostRequestAsync<T>(GetRestApiUrl(path), content);
        //}
    }
}
