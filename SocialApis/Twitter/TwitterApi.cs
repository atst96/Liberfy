using SocialApis.Twitter.Apis;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Utf8Json;
using System.Collections.Generic;

namespace SocialApis.Twitter
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class TwitterApi : IApi
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
            get => this._accessToken ?? string.Empty;
            internal set => this._accessToken = value;
        }

        public string AccessTokenSecret
        {
            get => this._accessTokenSecret ?? string.Empty;
            internal set => this._accessTokenSecret = value;
        }

        public string ApiToken => this._accessToken ?? string.Empty;
        public string ApiTokenSecret => this._accessTokenSecret ?? string.Empty;

        private TwitterApi() { }

        public TwitterApi(string consumerKey, string consumerSecret, string accessToken = null, string accessTokenSecret = null) : this()
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

        private const string RestApiBaseUrl = "https://api.twitter.com/1.1/";

        private static string GetRestApiUrl(string path)
        {
            return string.Concat(RestApiBaseUrl, path, ".json");
        }

        private Task<T> SendRequest<T>(string method, string endpoint, IQuery query = null) where T : class
        {
            return this.SendRequest<T>(WebUtility.CreateOAuthRequest(method, endpoint, this, query));
        }

        internal async Task<T> SendRequest<T>(HttpWebRequest request) where T : class
        {
            try
            {
                using var response = await request.GetResponseAsync().ConfigureAwait(false);
                var stream = response.GetResponseStream();
                var obj = await JsonUtility.DeserializeAsync<T>(stream).ConfigureAwait(false);

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

        internal Task<T> ApiGetRequestAsync<T>(string endpoint, IQuery query) where T : class
        {
            return this.SendRequest<T>(HttpMethods.GET, endpoint, query);
        }

        internal Task<T> ApiPostRequestAsync<T>(string endpoint, IQuery query = null) where T : class
        {
            return this.SendRequest<T>(HttpMethods.POST, endpoint, query);
        }


        internal Task<T> RestApiGetRequestAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.ApiGetRequestAsync<T>(GetRestApiUrl(path), query);
        }

        internal Task<T> RestApiPostRequestAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.ApiPostRequestAsync<T>(GetRestApiUrl(path), query);
        }
    }
}
