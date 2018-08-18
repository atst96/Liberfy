using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class NotificationColumn : StatusColumnBase
    {
        public NotificationColumn(AccountBase account)
            : base(account, ColumnType.Notification, "Notification")
        {
        }
    }
}
