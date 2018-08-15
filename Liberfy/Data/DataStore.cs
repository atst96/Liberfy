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
        private bool _isAddCollectionMode = false;

        private readonly object _userLockObj = new object();
        private readonly object _statusLockObj = new object();

        private static DataStore _twitter;
        public static DataStore Twitter => _twitter ?? (_twitter = new DataStore());

        private static DataStore _mastodon;
        public static DataStore Mastodon => _mastodon ?? (_mastodon = new DataStore());

        public ConcurrentDictionary<long, StatusInfo> Statuses { get; }

        public ConcurrentDictionary<long, UserInfo> Users { get; }

        public DataStore()
        {
            this.Statuses = new ConcurrentDictionary<long, StatusInfo>();
            this.Users = new ConcurrentDictionary<long, UserInfo>();

            BindingOperations.EnableCollectionSynchronization(this.Statuses, this._statusLockObj);
            BindingOperations.EnableCollectionSynchronization(this.Users, this._userLockObj);
        }

        public void BeginAddCollectionMode()
        {
            this._isAddCollectionMode = true;
        }

        public void EndAddCollectionMode()
        {
            this._isAddCollectionMode = false;
        }

        public UserInfo UserAddOrUpdate(IAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (account.Id.HasValue)
            {
                UserInfo userInfo;

                if (this.Users.TryGetValue(account.Id.Value, out userInfo))
                {
                    return this._isAddCollectionMode
                        ? userInfo
                        : userInfo.Update(account);
                }
                else
                {
                    userInfo = new UserInfo(account);
                    this.Users.TryAdd(account.Id.Value, userInfo);
                }

                return userInfo;
            }

            throw new ArgumentNullException("Account.Id");
        }

        public StatusInfo StatusAddOrUpdate(IStatus status)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));

            StatusInfo statusInfo;

            if (this.Statuses.TryGetValue(status.Id, out statusInfo))
            {
                return this._isAddCollectionMode
                    ? statusInfo
                    : statusInfo.Update(status);
            }
            else
            {
                statusInfo = StatusInfo.Create(status);
                this.Statuses.TryAdd(status.Id, statusInfo);

                return statusInfo;
            }
        }
    }
}
