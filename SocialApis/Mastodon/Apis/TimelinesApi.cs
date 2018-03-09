using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class TimelinesApi : TokenApiBase
    {
        internal TimelinesApi(Tokens tokens) : base(tokens) { }

        public Task<Status[]> Home(Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Status[]>("timelines/home", query);
        }

        public Task<Status[]> Public(Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Status[]>("timelines/public", query);
        }

        public Task<Status[]> Tag(string hashtag, Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Status[]>($"timelines/tag/{ OAuthHelper.UrlEncode(hashtag) }", query);
        }

        public Task<Status[]> Tag(long listId, Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<Status[]>($"timelines/list/{ listId }", query);
        }
    }
}
