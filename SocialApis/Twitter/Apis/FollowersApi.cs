using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    public class FollowersApi : TokenApiBase
    {
        internal FollowersApi(Tokens tokens) : base(tokens) { }

        public Task<CursoredIdsResponse> Ids()
        {
            return this.Tokens.GetRequestRestApiAsync<CursoredIdsResponse>("followers/ids");
        }

        public Task<CursoredIdsResponse> Ids(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };
            return this.Tokens.GetRequestRestApiAsync<CursoredIdsResponse>("followers/ids", query);
        }

        public Task<CursoredIdsResponse> Ids(Query query)
        {
            return this.Tokens.GetRequestRestApiAsync<CursoredIdsResponse>("followers/ids", query);
        }

        public Task<CursoredUsersResponse> List()
        {
            return this.Tokens.GetRequestRestApiAsync<CursoredUsersResponse>("followers/list");
        }

        public Task<CursoredUsersResponse> List(Query query)
        {
            return this.Tokens.GetRequestRestApiAsync<CursoredUsersResponse>("followers/list", query);
        }
    }
}
