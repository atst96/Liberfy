using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public class MutesApi : TokenApiBase
    {
        public MutesApi(Tokens tokens) : base(tokens) { }

        public Task<Account[]> GetMutes(Query query = null)
        {
            return this.Tokens.PostRequestRestApiAsync<Account[]>("mutes", query);
        }
    }
}
