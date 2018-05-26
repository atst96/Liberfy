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
		private VirtualizingPanel _itemsPanel;
		private Thumb _scrollBarThumb;

        public double ItemWidth { get; private set; }

		public TimelineView() : base()
		{
			this.Loaded += OnLoaded;
		}

        private void OnLoaded(object sender, RoutedEventArgs e)
		{
			this._scrollContainer = this.FindVisualChild<ScrollViewer>();
			this._itemsPanel = this._scrollContainer.FindVisualChild<VirtualizingPanel>();

			this._itemsPanel.SizeChanged += this.OnItemsPanelSizeChanged;

            this.ItemWidth = this._itemsPanel.ActualWidth;
		}

		private void OnItemsPanelSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if(e.WidthChanged)
			{
                this.ItemWidth = e.NewSize.Width;
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
