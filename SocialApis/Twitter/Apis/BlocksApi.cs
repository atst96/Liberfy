using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class BlocksApi : ApiBase
    {
        internal BlocksApi(TwitterApi tokens) : base(tokens)
        {
        }

        public Task<CursoredIdsResponse> Ids()
        {
            return this.Api.GetRequestRestApiAsync<CursoredIdsResponse>("blocks/ids");
        }

        public Task<CursoredIdsResponse> Ids(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };
            return this.Api.GetRequestRestApiAsync<CursoredIdsResponse>("blocks/ids", query);
        }

        public Task<CursoredUsersResponse> List()
        {
            return this.Api.GetRequestRestApiAsync<CursoredUsersResponse>("blocks/list");
        }

        public Task<CursoredUsersResponse> List(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };
            return this.Api.GetRequestRestApiAsync<CursoredUsersResponse>("blocks/list", query);
        }

        public Task<UserResponse> Create(long userId)
        {
            var q = new Query { ["user_id"] = userId };

            return this.Api.PostRequestRestApiAsync<UserResponse>("blocks/create", q);
        }

        public Task<UserResponse> Create(long userId, Query query)
        {
            var q = query?.Clone() ?? new Query();
            q["user_id"] = userId;

            return this.Api.PostRequestRestApiAsync<UserResponse>("blocks/create", q);
        }

        public Task<UserResponse> Destroy(long userId)
        {
            var q = new Query { ["user_id"] = userId };

            return this.Api.PostRequestRestApiAsync<UserResponse>("blocks/destroy", q);
        }

        public Task<UserResponse> Destroy(long userId, Query query)
        {
            var q = query?.Clone() ?? new Query();
            q["user_id"] = userId;

            return this.Api.PostRequestRestApiAsync<UserResponse>("blocks/destroy", q);
        }
    }
}
