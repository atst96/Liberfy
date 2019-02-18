using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class InstancesApi : ApiBase
    {
        internal InstancesApi(MastodonApi tokens) : base(tokens) { }

        public Task<Instance> GetInstance()
        {
            return this.Api.ApiGetRequestAsync<Instance>("instance");
        }

        public Task<Emoji[]> CustomEmojis()
        {
            return this.Api.ApiGetRequestAsync<Emoji[]>("custom_emojis");
        }
    }
}
