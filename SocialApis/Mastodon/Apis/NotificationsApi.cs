using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class NotificationsApi : ApiBase
    {
        public NotificationsApi(MastodonApi tokens) : base(tokens) { }

        public Task<Notification[]> GetNotifications(IQuery query = null)
        {
            return this.Api.RestApiGetRequestAsync<Notification[]>("notifications", query);
        }

        public Task<Notification[]> GetNotifications(NotificationType excludeType, IQuery query = null)
        {
            var _query = new Query(query);
            _query["exclude_type"] = excludeType;

            return this.Api.RestApiGetRequestAsync<Notification[]>("notifications", _query);
        }

        public Task<Notification[]> GetNotification(long id)
        {
            return this.Api.RestApiGetRequestAsync<Notification[]>($"notifications/{ id }");
        }

        public Task ClearNotifications()
        {
            return this.Api.RestApiPostRequestAsync("notifications/clear");
        }

        public Task DismissNotification(long notificationId)
        {
            var query = new Query { ["id"] = notificationId };
            return this.Api.RestApiPostRequestAsync("notifications/dismiss", query);
        }
    }
}
