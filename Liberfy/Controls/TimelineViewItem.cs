using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
    internal sealed class TimelineViewItem : ListBoxItem
    {
        private TimelineView _container;

        public bool IsRetweetedItem
        {
            get { return (bool)this.GetValue(TimelineViewItem.IsRetweetedItemProperty); }
            set { this.SetValue(TimelineViewItem.IsRetweetedItemProperty, value); }
        }

        public static readonly DependencyProperty IsRetweetedItemProperty =
            DependencyProperty.Register(nameof(TimelineViewItem.IsRetweetedItem), typeof(bool), typeof(TimelineViewItem), new PropertyMetadata(false));

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (this._container == null)
                this._container = ItemsControl.ItemsControlFromItemContainer(this) as TimelineView;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            constraint.Width = this._container.ItemWidth;
            return base.MeasureOverride(constraint);
        }
    }
}
