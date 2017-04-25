using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
	class ListBoxEx : ListBox
	{
		public ListBoxEx() : base() { }

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new ListBoxItemEx();
		}
	}

	class ListBoxItemEx : ListBoxItem
	{
		public ListBoxItemEx() : base()
		{
			IsKeyboardFocusWithinChanged += kbFocusWithin;
		}

		private void kbFocusWithin(object sender, DependencyPropertyChangedEventArgs e)
		{
			if((bool)e.NewValue)
			{
				OnSelected(new RoutedEventArgs(SelectedEvent));
			}
		}
	}
}
