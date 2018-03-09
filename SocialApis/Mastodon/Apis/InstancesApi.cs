using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class InstancesApi : TokenApiBase
    {
        internal InstancesApi(Tokens tokens) : base(tokens) { }

        public Task<Instance> GetInstance()
        {
            return this.Tokens.GetRequestAsync<Instance>("instance");
        }

        public Task<Emoji[]> CustomEmojis()
        {
            return this.Tokens.GetRequestAsync<Emoji[]>("custom_emojis");
        }
    }
}
