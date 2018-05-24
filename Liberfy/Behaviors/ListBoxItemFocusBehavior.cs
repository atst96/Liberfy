using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Liberfy.Behaviors
{
	class ListBoxItemFocusBehavior : Behavior<ListBoxItem>
	{
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.IsKeyboardFocusWithinChanged += kbFocusWithinChanged;
		}

		protected override void OnDetaching()
		{
			AssociatedObject.IsKeyboardFocusWithinChanged -= kbFocusWithinChanged;
			base.OnDetaching();
		}

		private void kbFocusWithinChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			AssociatedObject.RaiseEvent(new RoutedEventArgs(ListBoxItem.SelectedEvent, AssociatedObject));
		}
	}
}
