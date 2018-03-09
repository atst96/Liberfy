using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class BlocksApi : TokenApiBase
    {
        internal BlocksApi(Tokens tokens) : base(tokens) { }

        public Task<Account[]> GetAll(Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Account[]>("blocks", query);
        }
    }
}
