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
        private Panel _container;

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (this._container == null)
                this._container = this.FindAncestor<VirtualizingPanel>();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            constraint.Width = this._container.ActualWidth;
            return base.MeasureOverride(constraint);
        }
    }
}
