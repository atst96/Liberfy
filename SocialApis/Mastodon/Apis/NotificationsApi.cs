using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class NotificationsApi : TokenApiBase
    {
        public NotificationsApi(Tokens tokens) : base(tokens) { }

        public Task<Notification[]> GetNotifications(NotificationType excludeType, IQuery query = null)
        {
            var _query = new Query(query);
            _query["exclude_type"] = excludeType;

            return this.Tokens.GetRequestRestApiAsync<Notification[]>("notifications", _query);
        }

        public Task<Notification[]> GetNotification(long id)
        {
            return this.Tokens.GetRequestRestApiAsync<Notification[]>($"notifications/{ id }");
        }

        public Task ClearNotifications()
        {
            return this.Tokens.PostRequestRestApiAsync("notifications/clear");
        }

        public Task DismissNotification(long notificationId)
        {
            var query = new Query { ["id"] = notificationId };
            return this.Tokens.PostRequestRestApiAsync("notifications/dismiss", query);
        }
    }
}
