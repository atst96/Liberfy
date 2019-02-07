using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Liberfy
{
    internal abstract class TimelineBase : NotificationObject
    {
        public static NotifiableCollection<ColumnBase> Columns { get; } = new NotifiableCollection<ColumnBase>();

        protected ColumnManager AccountColumns { get; }

        protected TimelineBase(IAccount account)
        {
            this.AccountColumns = new ColumnManager(account, Columns);
        }

        public abstract void Load();

        public abstract void Unload();
    }
}
