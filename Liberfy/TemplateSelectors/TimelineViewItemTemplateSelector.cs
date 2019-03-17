using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
    internal class TimelineViewItemTemplateSelector : StyleSelector
    {
        private static readonly App app = App.Instance;

        private static readonly Style GeneralTimelineViewItemStyle = app.TryFindResource<Style>("GeneralTimelineViewItemStyle");
        private static readonly Style StatusTimelineViewItemStyle = app.TryFindResource<Style>("StatusTimelineViewItemStyle");

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is StatusItem)
            {
                return StatusTimelineViewItemStyle;
            }
            else
            {
                return GeneralTimelineViewItemStyle;
            }
        }
    }
}
