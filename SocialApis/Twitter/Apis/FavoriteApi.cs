using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    public class FavoriteApi : TokenApiBase
    {
        internal FavoriteApi(Tokens tokens) : base(tokens) { }

        public Task<StatusResponse> Create(long statusId)
        {
            return Create(new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Create(Query query)
        {
            return this.Tokens.PostRequestRestApiAsync<StatusResponse>("favorites/create", query);
        }

        public Task<StatusResponse> Destroy(long statusId)
        {
            return Destroy(new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Destroy(Query query)
        {
            return this.Tokens.PostRequestRestApiAsync<StatusResponse>("favorites/destroy", query);
        }
    }
}
