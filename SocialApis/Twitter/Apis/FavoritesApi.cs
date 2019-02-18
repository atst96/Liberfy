using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class FavoritesApi : ApiBase
    {
        internal FavoritesApi(TwitterApi tokens) : base(tokens) { }

        public Task<StatusResponse> Create(long statusId)
        {
            return Create(new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Create(IQuery query)
        {
            return this.Api.RestApiPostRequestAsync<StatusResponse>("favorites/create", query);
        }

        public Task<StatusResponse> Destroy(long statusId)
        {
            return Destroy(new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Destroy(IQuery query)
        {
            return this.Api.RestApiPostRequestAsync<StatusResponse>("favorites/destroy", query);
        }

        public Task<ListedResponse<User>> List(long userId)
        {
            return this.List(new Query { ["user_id"] = userId });
        }

        public Task<ListedResponse<User>> List(string screenName)
        {
            return this.List(new Query { ["screen_name"] = screenName });
        }

        public Task<ListedResponse<User>> List(IQuery query)
        {
            return this.Api.RestApiGetRequestAsync<ListedResponse<User>>("favorites/list", query);
        }
    }
}
