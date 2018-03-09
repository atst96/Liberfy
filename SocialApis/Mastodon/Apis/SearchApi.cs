using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class SearchApi : TokenApiBase
    {
        internal SearchApi(Tokens tokens) : base(tokens) { }

        public Task<Results> Search(string q, Query query = null)
        {
            query = new Query { ["q"] = q } + query;
            return this.Tokens.GetRequestRestApiAsync<Results>("search");
        }
    }
}
