using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class SearchApi : ApiBase
    {
        internal SearchApi(TwitterApi tokens) : base(tokens) { }

        public Task<ListedResponse<Status>> Search(string text)
        {
            return this.Search(new Query { ["q"] = text });
        }

        public Task<ListedResponse<Status>> Search(IQuery query)
        {
            return this.Api.RestApiGetRequestAsync<ListedResponse<Status>>("search/tweets", query);
        }
    }
}
