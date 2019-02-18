using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class MutesApi : ApiBase
    {
        public MutesApi(MastodonApi tokens) : base(tokens) { }

        public Task<Account[]> GetMutes(IQuery query = null)
        {
            return this.Api.RestApiPostRequestAsync<Account[]>("mutes", query);
        }
    }
}
