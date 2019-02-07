using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Liberfy
{
    internal abstract class DataStoreBase<IAccount, IStatus>
    {
        public ConcurrentDictionary<long, UserInfo> Accounts { get; }
        public ConcurrentDictionary<long, StatusInfo> Statuses { get; }

        protected DataStoreBase()
        {
            this.Accounts = new ConcurrentDictionary<long, UserInfo>();
            this.Statuses = new ConcurrentDictionary<long, StatusInfo>();

            BindingOperations.EnableCollectionSynchronization(this.Accounts, new object());
            BindingOperations.EnableCollectionSynchronization(this.Statuses, new object());
        }

        public UserInfo RegisterAccount(IAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            long id = this.GetAccountId(account);

            UserInfo info;

            if (this.Accounts.TryGetValue(id, out info))
            {
                if (App.Status.IsAccountLoaded)
                {
                    this.UpdateAccountInfo(info, account);
                }
            }
            else
            {
                info = CreateAccountInfo(account);
                this.Accounts.TryAdd(id, info);
            }

            return info;
        }

        protected abstract long GetAccountId(IAccount account);

        protected abstract void UpdateAccountInfo(UserInfo info, IAccount account);

        protected abstract UserInfo CreateAccountInfo(IAccount account);

        public StatusInfo RegisterStatus(IStatus status)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));

            long id = this.GetStatusId(status);

            StatusInfo info;

            if (this.Statuses.TryGetValue(id, out info))
            {
                if (App.Status.IsAccountLoaded)
                {
                    this.UpdateStatusInfo(info, status);
                }
            }
            else
            {
                info = CreateStatusInfo(status);
                this.Statuses.TryAdd(id, info);
            }

            return info;
        }

        protected abstract long GetStatusId(IStatus status);

        protected abstract void UpdateStatusInfo(StatusInfo info, IStatus status);

        protected abstract StatusInfo CreateStatusInfo(IStatus status);
    }
}
