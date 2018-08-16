using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class DirectMessageApi : TokenApiBase
    {
        internal DirectMessageApi(Tokens tokens) : base(tokens) { }

        public Task<Responses.DirectMessageListResponse> List()
        {
            return this.Tokens.GetRequestRestApiAsync<Responses.DirectMessageListResponse>("direct_messages/events/list");
        }

        public Task<Responses.DirectMessageListResponse> List(int? count = null, long? cursor = null)
        {
            var query = new Query();

            if (count.HasValue)
                query["count"] = count.Value;

            if (cursor.HasValue)
                query["cursor"] = cursor.Value;

            return this.Tokens.GetRequestRestApiAsync<Responses.DirectMessageListResponse>("direct_messages/events/list", query);
        }

        public Task<DirectMessageSingleResponse> Show(long id)
        {
            var query = new Query
            {
                ["id"] = id,
            };

            return this.Tokens.GetRequestRestApiAsync<DirectMessageSingleResponse>("direct_messages/events/show", query);
        }
    }
}
