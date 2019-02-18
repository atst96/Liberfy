using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class DirectMessageApi : ApiBase
    {
        internal DirectMessageApi(TwitterApi tokens) : base(tokens) { }

        public Task<Responses.DirectMessageListResponse> List()
        {
            return this.Api.RestApiGetRequestAsync<Responses.DirectMessageListResponse>("direct_messages/events/list");
        }

        public Task<Responses.DirectMessageListResponse> List(int? count = null, long? cursor = null)
        {
            var query = new Query();

            if (count.HasValue)
                query["count"] = count.Value;

            if (cursor.HasValue)
                query["cursor"] = cursor.Value;

            return this.Api.RestApiGetRequestAsync<Responses.DirectMessageListResponse>("direct_messages/events/list", query);
        }

        public Task<DirectMessageSingleResponse> Show(long id)
        {
            var query = new Query
            {
                ["id"] = id,
            };

            return this.Api.RestApiGetRequestAsync<DirectMessageSingleResponse>("direct_messages/events/show", query);
        }
    }
}
