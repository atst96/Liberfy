using Liberfy.Settings;
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
        public ConcurrentDictionary<long, IUserInfo<IAccount>> Accounts { get; }
        public ConcurrentDictionary<long, IStatusInfo<IStatus>> Statuses { get; }

        protected DataStoreBase()
        {
            this.Accounts = new ConcurrentDictionary<long, IUserInfo<IAccount>>();
            this.Statuses = new ConcurrentDictionary<long, IStatusInfo<IStatus>>();

            BindingOperations.EnableCollectionSynchronization(this.Accounts, new object());
            BindingOperations.EnableCollectionSynchronization(this.Statuses, new object());
        }

        public abstract IUserInfo<IAccount> GetAccount(AccountSettingBase item);

        public abstract IUserInfo<IAccount> RegisterAccount(IAccount account);

        public abstract IStatusInfo<IStatus> RegisterStatus(IStatus status);
    }
}
