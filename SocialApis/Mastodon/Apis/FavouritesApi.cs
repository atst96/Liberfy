using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class FavouritesApi : TokenApiBase
    {
        internal FavouritesApi(Tokens tokens) : base(tokens) { }

        public Task<Status[]> Favourites(Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Status[]>("favourites", query);
        }
    }
}
