using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class StatusesApi : ApiBase
    {
        internal StatusesApi(MastodonApi tokens) : base(tokens) { }

        public Task<Status> GetStatus(long statusId)
        {
            return this.Api.ApiGetRequestAsync<Status>($"statuses/{ statusId }");
        }

        public Task<Context> GetContext(long statusId)
        {
            return this.Api.ApiGetRequestAsync<Context>($"statuses/{ statusId }/context");
        }

        public Task<Card> GetCard(long statusId)
        {
            return this.Api.ApiGetRequestAsync<Card>($"statuses/{ statusId }/context");
        }

        public Task<Account[]> GetRebloggedBy(long statusId, IQuery query = null)
        {
            return this.Api.ApiGetRequestAsync<Account[]>($"statuses/{ statusId }/reblogged_by", query);
        }

        public Task<Account[]> GetFavouritedBy(long statusId, IQuery query = null)
        {
            return this.Api.ApiGetRequestAsync<Account[]>($"statuses/{ statusId }/favourited_by", query);
        }

        public Task<Status> Post(IQuery query)
        {
            return this.Api.RestApiPostRequestAsync<Status>("statuses", query);
        }

        public Task Delete(long statusId)
        {
            var req = this.Api.CreateRestApiDeleteRequest($"statuses/{ statusId }");
            return this.Api.SendRequest(req);
        }

        public Task<Status> Reblog(long statusId)
        {
            return this.Api.RestApiPostRequestAsync<Status>($"statuses/{ statusId }/reblog");
        }

        public Task<Status> Unreblog(long statusId)
        {
            return this.Api.RestApiPostRequestAsync<Status>($"statuses/{ statusId }/unreblog");
        }

        public Task<Status> Favourite(long statusId)
        {
            return this.Api.RestApiPostRequestAsync<Status>($"statuses/{ statusId }/favourite");
        }

        public Task<Status> Unfavourite(long statusId)
        {
            return this.Api.RestApiPostRequestAsync<Status>($"statuses/{ statusId }/unfavourite");
        }

        public Task<Status> Pin(long statusId)
        {
            return this.Api.RestApiPostRequestAsync<Status>($"statuses/{ statusId }/pin");
        }

        public Task<Status> Unpin(long statusId)
        {
            return this.Api.RestApiPostRequestAsync<Status>($"statuses/{ statusId }/unpin");
        }

        public Task<Status> Mute(long statusId)
        {
            return this.Api.RestApiPostRequestAsync<Status>($"statuses/{ statusId }/mute");
        }

        public Task<Status> Unmute(long statusId)
        {
            return this.Api.RestApiPostRequestAsync<Status>($"statuses/{ statusId }/unmute");
        }
    }
}
