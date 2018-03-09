using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class DomainBlocksApi : TokenApiBase
    {
        internal DomainBlocksApi(Tokens tokens) : base(tokens) { }

        public Task<string[]> GetDomains(Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<string[]>("domain_blocks", query);
        }

        public async Task BlockDomain(string domain)
        {
            var query = new Query { ["domain"] = OAuthHelper.UrlEncode(domain) };
            var req = this.Tokens.CreatePostRequester("domain_blocks", query);
            await WebUtility.SendRequestVoid(req);
        }

        public async Task UnblockDomain(string domain)
        {
            var query = new Query { ["domain"] = domain };
            var req = this.Tokens.CreateRequester("domain_blocks", query, "DELETE");
            await WebUtility.SendRequestVoid(req);
        }
    }
}
