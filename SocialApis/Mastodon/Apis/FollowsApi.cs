using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class FollowsApi : ApiBase
    {
        internal FollowsApi(MastodonApi tokens) : base(tokens) { }

        public Task<Account[]> GetFollows(string uri)
        {
            var query = new Query { ["uri"] = uri };
            return this.Api.GetRequestRestApiAsync<Account[]>("follows", query);
        }
    }
}
