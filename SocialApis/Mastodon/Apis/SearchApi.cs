using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class SearchApi : TokenApiBase
    {
        internal SearchApi(Tokens tokens) : base(tokens) { }

        public Task<Results> Search(string q, IQuery query = null)
        {
            var _q = new Query(query);
            _q["q"] = q;

            return this.Tokens.GetRequestRestApiAsync<Results>("search", _q);
        }
    }
}
