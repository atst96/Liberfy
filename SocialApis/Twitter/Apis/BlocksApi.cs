using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class BlocksApi : ApiBase
    {
        internal BlocksApi(TwitterApi tokens) : base(tokens)
        {
        }

        public Task<CursoredIdsResponse> Ids()
        {
            return this.Api.RestApiGetRequestAsync<CursoredIdsResponse>("blocks/ids");
        }

        public Task<CursoredIdsResponse> Ids(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };

            return this.Api.RestApiGetRequestAsync<CursoredIdsResponse>("blocks/ids", query);
        }

        public Task<CursoredUsersResponse> List()
        {
            return this.Api.RestApiGetRequestAsync<CursoredUsersResponse>("blocks/list");
        }

        public Task<CursoredUsersResponse> List(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };

            return this.Api.RestApiGetRequestAsync<CursoredUsersResponse>("blocks/list", query);
        }

        public Task<UserResponse> Create(long userId)
        {
            var query = new Query { ["user_id"] = userId };

            return this.Api.RestApiPostRequestAsync<UserResponse>("blocks/create", query);
        }

        public Task<UserResponse> Create(Query query)
        {
            return this.Api.RestApiPostRequestAsync<UserResponse>("blocks/create", query);
        }

        public Task<UserResponse> Destroy(long userId)
        {
            var query = new Query { ["user_id"] = userId };

            return this.Api.RestApiPostRequestAsync<UserResponse>("blocks/destroy", query);
        }

        public Task<UserResponse> Destroy(Query query)
        {
            return this.Api.RestApiPostRequestAsync<UserResponse>("blocks/destroy", query);
        }
    }
}
