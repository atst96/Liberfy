using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class FollowsApi : TokenApiBase
    {
        internal FollowsApi(Tokens tokens) : base(tokens) { }

        public Task<Account[]> GetFollows(string uri)
        {
            var query = new Query { ["uri"] = uri };
            return this.Tokens.GetRequestRestApiAsync<Account[]>("follows", query);
        }
    }
}
