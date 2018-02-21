using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    public class StatusesApi : TokenApiBase
    {
        internal StatusesApi(Tokens tokens) : base(tokens) { }

        #region Post, retrieve and engage with Tweets

        public Task<StatusResponse> Update(string status)
        {
            return Update(new Query { ["status"] = status });
        }

        public Task<StatusResponse> Update(Query query)
        {
            return this.Tokens.PostRequestRestApiAsync<StatusResponse>("statuses/update", query);
        }

        public Task<StatusResponse> Destroy(long statusId)
        {
            return this.Tokens.PostRequestRestApiAsync<StatusResponse>("statuses/destroy", new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Show(long statusId)
        {
            return Show(new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Show(Query query)
        {
            return this.Tokens.GetRequestRestApiAsync<StatusResponse>("statuses/show", query);
        }

        public Task<ListedResponse<Status>> Lookup(long[] statusIds)
        {
            return Lookup(new Query { ["id"] = statusIds });
        }

        public Task<ListedResponse<Status>> Lookup(Query query)
        {
            return this.Tokens.GetRequestRestApiAsync<ListedResponse<Status>>("statuses/lookup", query);
        }

        public Task<StatusResponse> Retweet(long statusId)
        {
            return Retweet(new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Retweet(Query query)
        {
            return this.Tokens.PostRequestRestApiAsync<StatusResponse>("statuses/retweet", query);
        }

        public Task<StatusResponse> Unretweet(long statusId)
        {
            return Unretweet(new Query { ["id"] = statusId });
        }

        public Task<StatusResponse> Unretweet(Query query)
        {
            return this.Tokens.PostRequestRestApiAsync<StatusResponse>("statuses/unretweet", query);
        }

        public Task<ListedResponse<Status>> Retweets(long statusId)
        {
            return Retweets(new Query { ["id"] = statusId });
        }

        public Task<ListedResponse<Status>> Retweets(long statusId, int count)
        {
            return Retweets(new Query { ["id"] = statusId, ["count"] = count });
        }

        public Task<ListedResponse<Status>> Retweets(Query query)
        {
            return this.Tokens.GetRequestRestApiAsync<ListedResponse<Status>>("statuses/retweets", query);
        }

        public Task<ListedResponse<Status>> RetweetsOfMe(Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<ListedResponse<Status>>("statuses/retweets_of_me", query);
        }

        public Task<ListedResponse<long>> RetweetersIds(long statusId)
        {
            return RetweetersIds(new Query { ["id"] = statusId });
        }

        public Task<ListedResponse<long>> RetweetersIds(long statusId, int count)
        {
            return RetweetersIds(new Query { ["id"] = statusId, ["count"] = count });
        }

        public Task<ListedResponse<long>> RetweetersIds(Query query)
        {
            return this.Tokens.GetRequestRestApiAsync<ListedResponse<long>>("statuses/retweeters_ids", query);
        }

        #endregion Post, retrieve and engage with Tweets

        #region Get Tweet timelines

        public Task<ListedResponse<Status>> HomeTimeline(Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<ListedResponse<Status>>("statuses/home_timeline", query);
        }

        public Task<ListedResponse<Status>> MentionsTimeline(Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<ListedResponse<Status>>("statuses/mentions_timeline", query);
        }

        public Task<ListedResponse<Status>> UserTimeline(Query query = null)
        {
            return this.Tokens.GetRequestRestApiAsync<ListedResponse<Status>>("statuses/user_timeline", query);
        }

        #endregion Get Tweet timelines
    }
}
