using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class SearchApi : ApiBase
    {
        internal SearchApi(MastodonApi tokens) : base(tokens) { }

        public Task<Results> Search(IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<Results>("search", query);
        }
    }
}
