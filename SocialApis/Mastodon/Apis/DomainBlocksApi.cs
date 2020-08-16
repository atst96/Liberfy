using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialApis.Utils;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class DomainBlocksApi : ApiBase
    {
        internal DomainBlocksApi(MastodonApi tokens) : base(tokens) { }

        public Task<string[]> GetDomains(Query query = null)
        {
            return this.Api.RestApiGetRequestAsync<string[]>("domain_blocks", query);
        }

        public Task BlockDomain(string domain)
        {
            var query = new Query { ["domain"] = WebUtil.UrlEncode(domain) };
            return this.Api.RestApiPostRequestAsync("domain_blocks", query);
        }

        public Task UnblockDomain(string domain)
        {
            var query = new Query { ["domain"] = WebUtil.UrlEncode(domain) };
            return this.Api.RestApiDeleteRequestAsync("domain_blocks", query);
        }
    }
}
