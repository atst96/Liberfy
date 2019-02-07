using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class InstancesApi : ApiBase
    {
        internal InstancesApi(MastodonApi tokens) : base(tokens) { }

        public Task<Instance> GetInstance()
        {
            return this.Api.GetRequestAsync<Instance>("instance");
        }

        public Task<Emoji[]> CustomEmojis()
        {
            return this.Api.GetRequestAsync<Emoji[]>("custom_emojis");
        }
    }
}
