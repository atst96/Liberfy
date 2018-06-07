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
        private static double _cachedItemWidth = 0.0d;

        private double _itemWidth = _cachedItemWidth;
        public double ItemWidth
        {
            get => this._itemWidth;
            set
            {
                if (value != 0 && value != this._itemWidth)
                {
                    this._itemWidth = value;
                    _cachedItemWidth = value;
                }
            }
        }

        private ScrollViewer _scrollContainer;
        private Panel _itemsPanel;
        private Thumb _scrollBarThumb;

        public TimelineView() : base()
        {
            this.Loaded += TimelineView_Loaded;
        }

        private void TimelineView_Loaded(object sender, RoutedEventArgs e)
        {
            this._scrollContainer = this.FindVisualChild<ScrollViewer>();
            this._itemsPanel = this._scrollContainer.FindVisualChild<VirtualizingPanel>();

            this.ItemWidth = _itemsPanel.ActualWidth;

            this._itemsPanel.SizeChanged += _itemsPanel_SizeChanged;
        }

        private void _itemsPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                this.ItemWidth = e.NewSize.Width;
                this._itemsPanel.UpdateLayout();
            }
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
