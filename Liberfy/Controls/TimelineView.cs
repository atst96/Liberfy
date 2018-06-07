using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Liberfy
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(TimelineViewItem))]
    internal sealed class TimelineView : ListBox
    {
        private ScrollViewer _scrollContainer;
        private Thumb _scrollBarThumb;

        public TimelineView() : base()
        {
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TimelineViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TimelineViewItem();
        }
    }
}
