using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class DomainBlocksApi : ApiBase
    {
        internal DomainBlocksApi(MastodonApi tokens) : base(tokens) { }

        public Task<string[]> GetDomains(Query query = null)
        {
            return this.Api.GetRequestRestApiAsync<string[]>("domain_blocks", query);
        }

        public async Task BlockDomain(string domain)
        {
            var query = new Query { ["domain"] = HttpHelper.UrlEncode(domain) };
            var req = this.Api.CreatePostRequester("domain_blocks", query);
            await WebUtility.SendRequestVoid(req);
        }

        public async Task UnblockDomain(string domain)
        {
            var query = new Query { ["domain"] = domain };
            var req = this.Api.CreateRequester("domain_blocks", query, "DELETE");
            await WebUtility.SendRequestVoid(req);
        }
    }
}
