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

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            this._container = ItemsControl.ItemsControlFromItemContainer(this) as TimelineView;
        }

        private double? _cachedWidth;
        private Size? _cachedSize;

        protected override Size MeasureOverride(Size constraint)
        {
            double containerWidth = this._container.ItemWidth;
            if (this._cachedWidth != containerWidth)
            {
                constraint.Width = containerWidth;
                this._cachedWidth = containerWidth;
                this._cachedSize = base.MeasureOverride(constraint);
            }

            return this._cachedSize.Value;
        }

        public void ClearCachedLayout()
        {
            this._cachedWidth = null;
            this._cachedSize = null;
        }
    }
}
