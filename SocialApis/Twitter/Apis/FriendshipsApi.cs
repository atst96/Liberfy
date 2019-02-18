using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class FriendshipsApi : ApiBase
    {
        internal FriendshipsApi(TwitterApi tokens) : base(tokens) { }

        public Task<CursoredIdsResponse> Incoming()
        {
            return this.Api.RestApiGetRequestAsync<CursoredIdsResponse>("friendships/incoming");
        }

        public Task<CursoredIdsResponse> Incoming(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };
            return this.Api.RestApiGetRequestAsync<CursoredIdsResponse>("friendships/incoming", query);
        }

        public Task<ListedResponse<Friendship>> Lookup(params long[] ids)
        {
            var query = new Query { ["user_id"] = ids };
            return this.Api.RestApiGetRequestAsync<ListedResponse<Friendship>>("friendships/lookup", query);
        }

        public Task<ListedResponse<Friendship>> Lookup(params string[] screenNames)
        {
            var query = new Query { ["screen_name"] = screenNames };
            return this.Api.RestApiGetRequestAsync<ListedResponse<Friendship>>("friendship/lookup", query);
        }

        public Task<ListedResponse<long>> NoRetweetsIds()
        {
            return this.Api.RestApiGetRequestAsync<ListedResponse<long>>("friendships/no_retweets/ids");
        }

        public Task<CursoredIdsResponse> Outgoing()
        {
            return this.Api.RestApiGetRequestAsync<CursoredIdsResponse>("friendships/outgoing");
        }

        public Task<CursoredIdsResponse> Outgoing(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };
            return this.Api.RestApiGetRequestAsync<CursoredIdsResponse>("friendships/outgoing", query);
        }

        // TODO: friendships/show

        public Task<UserResponse> Create(string screenName)
        {
            return this.Create(screenName, true);
        }

        public Task<UserResponse> Create(string screenName, bool followNotification)
        {
            var query = new Query { ["screen_name"] = screenName, ["follow"] = followNotification };
            return this.Api.RestApiPostRequestAsync<UserResponse>("friendships/create", query);
        }

        public Task<UserResponse> Create(long userId)
        {
            return this.Create(userId, true);
        }

        public Task<UserResponse> Create(long userId, bool followNotification)
        {
            var query = new Query { ["user_id"] = userId, ["follow"] = followNotification };
            return this.Api.RestApiPostRequestAsync<UserResponse>("friendships/create", query);
        }

        public Task<UserResponse> Destroy(string screenName)
        {
            var query = new Query { ["screen_name"] = screenName };
            return this.Api.RestApiPostRequestAsync<UserResponse>("friendships/destroy", query);
        }

        public Task<UserResponse> Destroy(long userId)
        {
            var query = new Query { ["user_id"] = userId };
            return this.Api.RestApiPostRequestAsync<UserResponse>("friendships/destroy", query);
        }

        // TODO: friendships/update
    }
}
