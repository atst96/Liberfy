using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class FavouritesApi : ApiBase
    {
        internal FavouritesApi(MastodonApi tokens) : base(tokens) { }

        public Task<Status[]> Favourites(IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<Status[]>("favourites", query);
        }
    }
}
