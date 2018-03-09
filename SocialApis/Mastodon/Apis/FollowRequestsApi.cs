using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class FollowRequestsApi : TokenApiBase
    {
        internal FollowRequestsApi(Tokens tokens) : base(tokens) { }

        public Task<Account[]> GetFollowRequests(Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Account[]>("follow_requests", query);
        }

        public Task Authorize(long accountId)
        {
            var req = this.Tokens.CreatePostRequesterApi($"follow_requests/{ accountId }/authorize");
            return WebUtility.SendRequestVoid(req);
        }

        public Task Reject(long accountId)
        {
            var req = this.Tokens.CreatePostRequesterApi($"follow_requests/{ accountId }/reject");
            return WebUtility.SendRequestVoid(req);
        }
    }
}
