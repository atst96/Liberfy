using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class TimelinesApi : TokenApiBase
    {
        internal TimelinesApi(Tokens tokens) : base(tokens) { }

        public Task<Status[]> Home(IQuery query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Status[]>("timelines/home", query);
        }

        public Task<Status[]> Public(IQuery query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Status[]>("timelines/public", query);
        }

        public Task<Status[]> Tag(string hashtag, IQuery query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Status[]>($"timelines/tag/{ OAuthHelper.UrlEncode(hashtag) }", query);
        }

        public Task<Status[]> Tag(long listId, IQuery query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Status[]>($"timelines/list/{ listId }", query);
        }
    }
}
