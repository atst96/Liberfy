using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class FollowersApi : ApiBase
    {
        internal FollowersApi(TwitterApi tokens) : base(tokens) { }

        public Task<CursoredIdsResponse> Ids()
        {
            return this.Api.GetRequestRestApiAsync<CursoredIdsResponse>("followers/ids");
        }

        public Task<CursoredIdsResponse> Ids(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };
            return this.Api.GetRequestRestApiAsync<CursoredIdsResponse>("followers/ids", query);
        }

        public Task<CursoredIdsResponse> Ids(IQuery query)
        {
            return this.Api.GetRequestRestApiAsync<CursoredIdsResponse>("followers/ids", query);
        }

        public Task<CursoredUsersResponse> List()
        {
            return this.Api.GetRequestRestApiAsync<CursoredUsersResponse>("followers/list");
        }

        public Task<CursoredUsersResponse> List(IQuery query)
        {
            return this.Api.GetRequestRestApiAsync<CursoredUsersResponse>("followers/list", query);
        }
    }
}
