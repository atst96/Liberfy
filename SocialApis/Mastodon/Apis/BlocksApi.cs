using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class BlocksApi : ApiBase
    {
        internal BlocksApi(MastodonApi tokens) : base(tokens) { }

        public Task<Account[]> GetAll(IQuery query = null)
        {
            return this.Api.GetRequestRestApiAsync<Account[]>("blocks", query);
        }
    }
}
