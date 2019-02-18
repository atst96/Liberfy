using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class FollowsApi : ApiBase
    {
        internal FollowsApi(MastodonApi tokens) : base(tokens) { }

        public Task<Account[]> GetFollows(string uri)
        {
            var query = new Query { ["uri"] = uri };
            return this.Api.RestApiGetRequestAsync<Account[]>("follows", query);
        }
    }
}
