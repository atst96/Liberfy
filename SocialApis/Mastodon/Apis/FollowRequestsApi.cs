using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class FollowRequestsApi : ApiBase
    {
        internal FollowRequestsApi(MastodonApi tokens) : base(tokens) { }

        public Task<Account[]> GetFollowRequests(IQuery query = null)
        {
            return this.Api.GetRequestRestApiAsync<Account[]>("follow_requests", query);
        }

        public Task Authorize(long accountId)
        {
            var req = this.Api.CreatePostRequesterApi($"follow_requests/{ accountId }/authorize");
            return WebUtility.SendRequestVoid(req);
        }

        public Task Reject(long accountId)
        {
            var req = this.Api.CreatePostRequesterApi($"follow_requests/{ accountId }/reject");
            return WebUtility.SendRequestVoid(req);
        }
    }
}
