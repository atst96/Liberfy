using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    public class FriendsApi : TokenApiBase
    {
        internal FriendsApi(Tokens tokens) : base(tokens) { }

        public Task<CursoredIdsResponse> Ids()
        {
            return this.Tokens.GetRequestRestApiAsync<CursoredIdsResponse>("friends/ids");
        }

        public Task<CursoredIdsResponse> Ids(int cursor)
        {
            var query = new Query { ["cursor"] = cursor };
            return this.Tokens.GetRequestRestApiAsync<CursoredIdsResponse>("friends/ids", query);
        }

        public Task<CursoredIdsResponse> Ids(Query query)
        {
            return this.Tokens.GetRequestRestApiAsync<CursoredIdsResponse>("friends/ids", query);
        }

        public Task<CursoredUsersResponse> List()
        {
            return this.Tokens.GetRequestRestApiAsync<CursoredUsersResponse>("friends/list");
        }

        public Task<CursoredUsersResponse> List(Query query)
        {
            return this.Tokens.GetRequestRestApiAsync<CursoredUsersResponse>("friends/list", query);
        }
    }
}
