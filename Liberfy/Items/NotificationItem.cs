using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MastodonNotification = SocialApis.Mastodon.Notification;

namespace Liberfy
{
    internal class NotificationItem : IItem
    {
        public ItemType Type { get; }

        public long Id { get; }

        public DateTimeOffset CreatedAt { get; }

        public StatusInfo Status { get; }

        public IAccount Own { get; }

        public UserInfo Account { get; }

        public NotificationItem(MastodonNotification item, MastodonAccount account)
        {
            this.Id = account.Id;
            this.Own = account;
            this.CreatedAt = item.CreatedAt;

            switch (item.Type)
            {
                case SocialApis.Mastodon.NotificationType.Reblog:
                    this.Type = ItemType.RetweetActivity;
                    this.Status = account.DataStore.RegisterStatus(item.Status);
                    this.Account = account.DataStore.RegisterAccount(item.Account);
                    break;

                case SocialApis.Mastodon.NotificationType.Favourite:
                    this.Type = ItemType.FavoriteActivity;
                    this.Status = account.DataStore.RegisterStatus(item.Status);
                    this.Account = account.DataStore.RegisterAccount(item.Account);
                    break;

                case SocialApis.Mastodon.NotificationType.Follow:
                    this.Type = ItemType.FollowActivity;
                    this.Account = account.DataStore.RegisterAccount(item.Account);
                    break;

                case SocialApis.Mastodon.NotificationType.Mention:
                    throw new NotSupportedException();
            }
        }

        public bool IsAnimatable => throw new NotImplementedException();
    }
}
