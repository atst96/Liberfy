using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal abstract class TimelineBase : NotificationObject
    {
        public static NotifiableCollection<ColumnBase> Columns { get; } = new NotifiableCollection<ColumnBase>();

        public abstract void Load();

        public abstract void Unload();
    }
}
