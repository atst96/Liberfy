using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class FriendshipsApi : TokenApiBase
    {
        internal FriendshipsApi(Tokens tokens) : base(tokens) { }

        public Task<CursoredIdsResponse> Incoming()
        {
            return this.Tokens.GetRequestRestApiAsync<CursoredIdsResponse>("friendships/incoming");
        }

        public Task<CursoredIdsResponse> Incoming(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };
            return this.Tokens.GetRequestRestApiAsync<CursoredIdsResponse>("friendships/incoming", query);
        }

        public Task<ListedResponse<Friendship>> Lookup(params long[] ids)
        {
            var query = new Query { ["user_id"] = ids };
            return this.Tokens.GetRequestRestApiAsync<ListedResponse<Friendship>>("friendships/lookup", query);
        }

        public Task<ListedResponse<Friendship>> Lookup(params string[] screenNames)
        {
            var query = new Query { ["screen_name"] = screenNames };
            return this.Tokens.GetRequestRestApiAsync<ListedResponse<Friendship>>("friendship/lookup", query);
        }

        public Task<ListedResponse<long>> NoRetweetsIds()
        {
            return this.Tokens.GetRequestRestApiAsync<ListedResponse<long>>("friendships/no_retweets/ids");
        }

        public Task<CursoredIdsResponse> Outgoing()
        {
            return this.Tokens.GetRequestRestApiAsync<CursoredIdsResponse>("friendships/outgoing");
        }

        public Task<CursoredIdsResponse> Outgoing(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };
            return this.Tokens.GetRequestRestApiAsync<CursoredIdsResponse>("friendships/outgoing", query);
        }

        // TODO: friendships/show

        public Task<UserResponse> Create(string screenName)
        {
            return this.Create(screenName, true);
        }

        public Task<UserResponse> Create(string screenName, bool followNotification)
        {
            var query = new Query { ["screen_name"] = screenName, ["follow"] = followNotification };
            return this.Tokens.PostRequestRestApiAsync<UserResponse>("friendships/create", query);
        }

        public Task<UserResponse> Create(long userId)
        {
            return this.Create(userId, true);
        }

        public Task<UserResponse> Create(long userId, bool followNotification)
        {
            var query = new Query { ["user_id"] = userId, ["follow"] = followNotification };
            return this.Tokens.PostRequestRestApiAsync<UserResponse>("friendships/create", query);
        }

        public Task<UserResponse> Destroy(string screenName)
        {
            var query = new Query { ["screen_name"] = screenName };
            return this.Tokens.PostRequestRestApiAsync<UserResponse>("friendships/destroy", query);
        }

        public Task<UserResponse> Destroy(long userId)
        {
            var query = new Query { ["user_id"] = userId };
            return this.Tokens.PostRequestRestApiAsync<UserResponse>("friendships/destroy", query);
        }

        // TODO: friendships/update
    }
}
