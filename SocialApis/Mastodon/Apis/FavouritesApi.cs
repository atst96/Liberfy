using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class FavouritesApi : TokenApiBase
    {
        internal FavouritesApi(Tokens tokens) : base(tokens) { }

        public Task<Status[]> Favourites(IQuery query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Status[]>("favourites", query);
        }
    }
}
