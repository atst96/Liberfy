using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class FollowRequestsApi : ApiBase
    {
        internal FollowRequestsApi(MastodonApi tokens) : base(tokens) { }

        public Task<Account[]> GetFollowRequests(IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<Account[]>("follow_requests", query);
        }

        public Task Authorize(long accountId)
        {
            return this.Api.RestApiPostRequestAsync($"follow_requests/{ accountId }/authorize");
        }

        public Task Reject(long accountId)
        {
            return this.Api.RestApiPostRequestAsync($"follow_requests/{ accountId }/reject");
        }
    }
}
