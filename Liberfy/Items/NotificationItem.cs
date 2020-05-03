using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MastodonApi = SocialApis.Mastodon;
using MastodonNotification = SocialApis.Mastodon.Notification;

namespace Liberfy
{
    internal class NotificationItem : IItem
    {
        public ItemType Type { get; }

        public long Id { get; }

        public DateTimeOffset CreatedAt { get; }

        public IStatusInfo Status { get; }

        public IAccount Own { get; }

        public IUserInfo Account { get; }

        public NotificationItem(MastodonNotification item, MastodonAccount account)
        {
            this.Id = account.Id;
            this.Own = account;
            this.CreatedAt = item.CreatedAt;

            switch (item.Type)
            {
                case MastodonApi.NotificationTypes.Reblog:
                    this.Type = ItemType.RetweetActivity;
                    this.Status = account.DataStore.RegisterStatus(item.Status);
                    this.Account = account.DataStore.RegisterAccount(item.Account);
                    break;

                case MastodonApi.NotificationTypes.Favourite:
                    this.Type = ItemType.FavoriteActivity;
                    this.Status = account.DataStore.RegisterStatus(item.Status);
                    this.Account = account.DataStore.RegisterAccount(item.Account);
                    break;

                case MastodonApi.NotificationTypes.Follow:
                    this.Type = ItemType.FollowActivity;
                    this.Account = account.DataStore.RegisterAccount(item.Account);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public bool IsAnimatable => throw new NotImplementedException();
    }
}
