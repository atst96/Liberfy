using SocialApis.Twitter.Apis;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Utf8Json;
using System.Collections.Generic;

namespace SocialApis.Twitter
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

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

        public string ApiToken => this._accessToken ?? string.Empty;
        public string ApiTokenSecret => this._accessTokenSecret ?? string.Empty;

        private Tokens() { }

        public Tokens(string consumerKey, string consumerSecret, string accessToken = null, string accessTokenSecret = null) : this()
        {
            this._consumerKey = consumerKey;
            this._consumerSecret = consumerSecret;
            this._accessToken = accessToken;
            this._accessTokenSecret = accessTokenSecret;
        }

        private OAuthApi _oauth;
        public OAuthApi OAuth => this._oauth ?? (this._oauth = new OAuthApi(this));

        private AccountApi _account;
        public AccountApi Account => this._account ?? (this._account = new AccountApi(this));

        private StatusesApi _statuses;
        public StatusesApi Statuses => this._statuses ?? (this._statuses = new StatusesApi(this));

        private FavoritesApi _favorites;
        public FavoritesApi Favorites => this._favorites ?? (this._favorites = new FavoritesApi(this));

        private CollectionsApi _collections;
        public CollectionsApi Collections => this._collections ?? (this._collections = new CollectionsApi(this));

        private MediaApi _mediaApi;
        public MediaApi Media => this._mediaApi ?? (this._mediaApi = new MediaApi(this));

        private BlocksApi _blocks;
        public BlocksApi Blocks => this._blocks ?? (this._blocks = new BlocksApi(this));

        private MutesApi _mutes;
        public MutesApi Mutes => this._mutes ?? (this._mutes = new MutesApi(this));

        private UsersApi _users;
        public UsersApi Users => this._users ?? (this._users = new UsersApi(this));

        private FollowersApi _followers;
        public FollowersApi Followers => this._followers ?? (this._followers = new FollowersApi(this));

        private FriendsApi _friends;
        public FriendsApi Friends => this._friends ?? (this._friends = new FriendsApi(this));

        private FriendshipsApi _friendships;
        public FriendshipsApi Friendships => this._friendships ?? (this._friendships = new FriendshipsApi(this));

        private const string _restApiBaseUrl = "https://api.twitter.com/1.1/";

        internal HttpWebRequest CreateRequester(string endpoint, IQuery query = null, string method = "GET", bool autoSetting = true)
        {
            return WebUtility.CreateOAuthWebRequest(endpoint, this, query, method, autoSetting);
        }

        internal HttpWebRequest CreateGetRequester(string endpoint, IQuery query = null, bool autoSetting = true)
        {
            return this.CreateRequester(endpoint, query, "GET", autoSetting);
        }

        internal HttpWebRequest CreatePostRequester(string endpoint, IQuery query = null, bool autoSetting = true)
        {
            return this.CreateRequester(endpoint, query, "POST", autoSetting);
        }

        internal HttpWebRequest CreateRequesterApi(string path, IQuery query = null, string method = "GET", bool autoSetting = true)
        {
            return WebUtility.CreateOAuthWebRequest($"{_restApiBaseUrl}{path}.json", this, query, method, autoSetting);
        }

        internal HttpWebRequest CreateGetRequesterApi(string path, IQuery query = null, bool autoSetting = true)
        {
            return this.CreateRequesterApi(path, query, "GET", autoSetting);
        }

        internal HttpWebRequest CreatePostRequesterApi(string path, IQuery query = null, bool autoSetting = true)
        {
            return this.CreateRequesterApi(path, query, "POST", autoSetting);
        }

        private Task<T> SendRequest<T>(string endpoint, IQuery query = null, string method = "GET") where T : class
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
                    var str = webRes.GetResponseStream();

                    var wstr = str as System.Net.Sockets.NetworkStream;

                    var obj = await JsonSerializer.DeserializeAsync<T>(str, Utf8Json.Resolvers.StandardResolver.AllowPrivate);

                    if (obj is IRateLimit rObj)
                        rObj.RateLimit = RateLimit.FromHeaders(webRes.Headers);

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

        private Task<T> SendApiRequestAsync<T>(string path, IQuery query, string method) where T : class
        {
            return this.SendRequest<T>($"{_restApiBaseUrl}{path}.json", query, method);
        }

        internal Task<T> GetRequestAsync<T>(string endpoint, IQuery query) where T : class
        {
            return this.SendRequest<T>(endpoint, query, "GET");
        }

        internal Task<T> GetRequestRestApiAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequestAsync<T>(path, query, "GET");
        }

        internal Task<T> PostRequestAsync<T>(string endpoint, IQuery query = null) where T : class
        {
            return this.SendRequest<T>(endpoint, query, "POST");
        }

        internal Task<T> PostRequestRestApiAsync<T>(string path, IQuery query = null) where T : class
        {
            return this.SendApiRequestAsync<T>(path, query, "POST");
        }
    }
}
