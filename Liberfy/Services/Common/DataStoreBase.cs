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

        public abstract UserInfo RegisterAccount(IAccount account);

        public abstract StatusInfo RegisterStatus(IStatus status);
    }
}
