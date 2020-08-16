using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialApis.Utils;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class TimelinesApi : ApiBase
    {
        internal TimelinesApi(MastodonApi tokens) : base(tokens) { }

        public Task<Status[]> Home(IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<Status[]>("timelines/home", query);
        }

        public Task<Status[]> Public(IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<Status[]>("timelines/public", query);
        }

        public Task<Status[]> Tag(string hashtag, IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<Status[]>($"timelines/tag/{ WebUtil.UrlEncode(hashtag) }", query);
        }

        public Task<Status[]> Tag(long listId, IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<Status[]>($"timelines/list/{ listId }", query);
        }
    }
}
