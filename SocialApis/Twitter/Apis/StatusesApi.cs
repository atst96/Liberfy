using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class StatusesApi : ApiBase
    {
        internal StatusesApi(TwitterApi tokens) : base(tokens) { }

        #region Post, retrieve and engage with Tweets

        public Task<StatusResponse> Update(string status)
        {
            return Update(new Query { ["status"] = status });
        }

        public Task<StatusResponse> Update(IQuery query)
        {
            return this.Api.RestApiPostRequestAsync<StatusResponse>("statuses/update", query);
        }

        public Task<StatusResponse> Destroy(long statusId)
        {
            return this.Api.RestApiPostRequestAsync<StatusResponse>("statuses/destroy", new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Show(long statusId)
        {
            return Show(new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Show(IQuery query)
        {
            return this.Api.RestApiGetRequestAsync<StatusResponse>("statuses/show", query);
        }

        public Task<ListedResponse<Status>> Lookup(long[] statusIds)
        {
            return Lookup(new Query { ["id"] = statusIds });
        }

        public Task<ListedResponse<Status>> Lookup(IQuery query)
        {
            return this.Api.RestApiGetRequestAsync<ListedResponse<Status>>("statuses/lookup", query);
        }

        public Task<StatusResponse> Retweet(long statusId)
        {
            return Retweet(new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Retweet(IQuery query)
        {
            return this.Api.RestApiPostRequestAsync<StatusResponse>("statuses/retweet", query);
        }

        public Task<StatusResponse> Unretweet(long statusId)
        {
            return Unretweet(new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Unretweet(IQuery query)
        {
            return this.Api.RestApiPostRequestAsync<StatusResponse>("statuses/unretweet", query);
        }

        public Task<ListedResponse<Status>> Retweets(long statusId)
        {
            return Retweets(new Query { ["id"] = statusId });
        }

        public Task<ListedResponse<Status>> Retweets(long statusId, int count)
        {
            return Retweets(new Query { ["id"] = statusId, ["count"] = count });
        }

        public Task<ListedResponse<Status>> Retweets(IQuery query)
        {
            return this.Api.RestApiGetRequestAsync<ListedResponse<Status>>("statuses/retweets", query);
        }

        public Task<CursoredIdsResponse> RetweetersIds(long statusId)
        {
            return this.RetweetersIds(new Query { ["id"] = statusId });
        }

        public Task<CursoredIdsResponse> RetweetersIds(IQuery query)
        {
            return this.Api.RestApiGetRequestAsync<CursoredIdsResponse>("statuses/retweeters/ids", query);
        }

        public Task<ListedResponse<Status>> RetweetsOfMe(IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<ListedResponse<Status>>("statuses/retweets_of_me", query);
        }

        #endregion Post, retrieve and engage with Tweets

        #region Get Tweet timelines

        public Task<ListedResponse<Status>> HomeTimeline(IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<ListedResponse<Status>>("statuses/home_timeline", query);
        }

        public Task<ListedResponse<Status>> MentionsTimeline(IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<ListedResponse<Status>>("statuses/mentions_timeline", query);
        }

        public Task<ListedResponse<Status>> UserTimeline(IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<ListedResponse<Status>>("statuses/user_timeline", query);
        }

        #endregion Get Tweet timelines
    }
}
