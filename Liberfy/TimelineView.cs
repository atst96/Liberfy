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
	internal sealed class TimelineView : TreeView
	{
		private ScrollViewer _scrollContainer;
		private VirtualizingPanel _itemsPanel;
		private Thumb _scrollBarThumb;

		public TimelineView() : base()
		{
			this.Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_scrollContainer = this.FindVisualChild<ScrollViewer>();
			_itemsPanel = _scrollContainer.FindVisualChild<VirtualizingPanel>();

			_itemsPanel.SizeChanged += OnItemsPanelSizeChanged;
		}

		private void OnItemsPanelSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if(e.WidthChanged)
			{
				Resources["ItemWidth"] = e.NewSize.Width;
			}
		}
	}
}
