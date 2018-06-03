using SocialApis;
using SocialApis.Common;
using SocialApis.Twitter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Liberfy
{
    internal class DataStore
    {
        private static DataStore _twitter;
        public static DataStore Twitter => _twitter ?? (_twitter = new DataStore());

        private static DataStore _mastodon;
        public static DataStore Mastodon => _mastodon ?? (_mastodon = new DataStore());

        public DataStore()
        {
            this.Statuses = new ConcurrentDictionary<long, StatusInfo>();
            this.Users = new ConcurrentDictionary<long, UserInfo>();

            BindingOperations.EnableCollectionSynchronization(this.Statuses, new object());
            BindingOperations.EnableCollectionSynchronization(this.Users, new object());
        }

        public ConcurrentDictionary<long, StatusInfo> Statuses { get; }

        public ConcurrentDictionary<long, UserInfo> Users { get; }

        public UserInfo UserAddOrUpdate(ICommonAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (account.Id.HasValue)
            {
                return this.Users.AddOrUpdate(
                    account.Id.Value,
                    (id) => new UserInfo(account),
                    (id, info) => info.Update(account));
            }

            return null;
        }

        public StatusInfo StatusAddOrUpdate(ICommonStatus status)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));

            return this.Statuses.AddOrUpdate(
                status.Id,
                (id) => new StatusInfo(status),
                (id, info) => info.Update(status));
        }
    }
}
