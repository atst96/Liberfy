using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class NotificationColumn : StatusColumnBase
    {
        public NotificationColumn(Timeline timeline)
            : base(timeline, ColumnType.Notification, "Notification")
        {
            if (timeline != null)
            {
                timeline.OnNotificationsLoaded += OnNotificationLoaded;
            }
        }

        private void OnNotificationLoaded(object sender, IEnumerable<IItem> e)
        {
            if (sender is Timeline timeline)
            {
                timeline.OnNotificationsLoaded -= OnNotificationLoaded;
            }

            this.Items.Reset(e);
        }
    }
}
