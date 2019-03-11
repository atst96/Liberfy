using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
    internal class TimelineItemTemplateSelector : DataTemplateSelector
    {
        private static readonly App _app = App.Instance;

        private static readonly DataTemplate _statusItemTemplate = _app.TryFindResource<DataTemplate>("StatusItemTemplate");
        private static readonly IDictionary<ItemType, DataTemplate> _notificationItemTemplates = new Dictionary<ItemType, DataTemplate>
        {
            [ItemType.FavoriteActivity] = _app.TryFindResource<DataTemplate>("FavoriteNotificationItemTemplate"),
            [ItemType.RetweetActivity] = _app.TryFindResource<DataTemplate>("RetweetNotificationItemTemplate"),
            [ItemType.FollowActivity] = _app.TryFindResource<DataTemplate>("FollowNotificationItemTemplate"),
        };

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is StatusItem)
            {
                return _statusItemTemplate;
            }

            if (item is NotificationItem nItem)
            {
                if (_notificationItemTemplates.TryGetValue(nItem.Type, out var template))
                {
                    return template;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
