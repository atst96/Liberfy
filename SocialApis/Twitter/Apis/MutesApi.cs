using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class MutesApi : ApiBase
    {
        internal MutesApi(TwitterApi tokens) : base(tokens)
        {
        }

        public Task<CursoredIdsResponse> Ids()
        {
            return this.Api.RestApiGetRequestAsync<CursoredIdsResponse>("mutes/users/ids");
        }

        public Task<CursoredIdsResponse> Ids(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };
            return this.Api.RestApiGetRequestAsync<CursoredIdsResponse>("mutes/users/ids", query);
        }

        public Task<CursoredUsersResponse> List()
        {
            return this.Api.RestApiGetRequestAsync<CursoredUsersResponse>("mutes/users/list");
        }

        public Task<CursoredUsersResponse> List(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };
            return this.Api.RestApiGetRequestAsync<CursoredUsersResponse>("mutes/users/list", query);
        }

        public Task<UserResponse> Create(long userId)
        {
            var q = new Query { ["user_id"] = userId };

            return this.Api.RestApiPostRequestAsync<UserResponse>("mutes/users/create", q);
        }

        public Task<UserResponse> Create(long userId, IQuery query)
        {
            var q = new Query(query);
            q["user_id"] = userId;

            return this.Api.RestApiPostRequestAsync<UserResponse>("mutes/users/create", q);
        }

        public Task<UserResponse> Destroy(long userId)
        {
            var q = new Query { ["user_id"] = userId };

            return this.Api.RestApiPostRequestAsync<UserResponse>("mutes/users/destroy", q);
        }

        public Task<UserResponse> Destroy(long userId, IQuery query)
        {
            var q = new Query(query);
            q["user_id"] = userId;

            return this.Api.RestApiPostRequestAsync<UserResponse>("mutes/users/destroy", q);
        }
    }
}
