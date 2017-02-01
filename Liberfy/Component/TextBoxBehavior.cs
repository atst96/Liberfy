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
	class TextBoxBehavior : Behavior<TextBox>
	{
		public TextBoxBehavior() : base() { }

		protected override void OnAttached()
		{
			AssociatedObject.SelectionChanged += OnSelectionChanged;
			base.OnAttached();
		}

		protected override void OnDetaching()
		{
			AssociatedObject.SelectionChanged -= OnSelectionChanged;
			base.OnDetaching();
		}

		private void OnSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (SelectionStart != AssociatedObject.SelectionStart)
			{
				selectionChangedEventCalled = true;
				SelectionStart = AssociatedObject.SelectionStart;
			}

			if (SelectionLength != AssociatedObject.SelectionLength)
			{
				selectionChangedEventCalled = true;
				SelectionLength = AssociatedObject.SelectionLength;
			}
		}

		private bool selectionChangedEventCalled;


		public int SelectionStart
		{
			get { return (int)GetValue(SelectionStartProperty); }
			set { SetValue(SelectionStartProperty, value); }
		}

		public int SelectionLength
		{
			get { return (int)GetValue(SelectionLengthProperty); }
			set { SetValue(SelectionLengthProperty, value); }
		}


		public static readonly DependencyProperty SelectionStartProperty =
			DependencyProperty.Register("SelectionStart", 
				typeof(int), typeof(TextBoxBehavior), 
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectionStartChanged));

		private static readonly DependencyProperty SelectionLengthProperty =
			DependencyProperty.Register("SelectionLength",
				typeof(int), typeof(TextBoxBehavior),
				new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectionLengthChanged));


		private static void SelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var b = ((TextBoxBehavior)d);

			if (!b.selectionChangedEventCalled)
				b.AssociatedObject.SelectionStart = (int)e.NewValue;
			else
				b.selectionChangedEventCalled = false;
		}

		private static void SelectionLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var b = ((TextBoxBehavior)d);

			if (!b.selectionChangedEventCalled)
				b.AssociatedObject.SelectionLength = (int)e.NewValue;
			else
				b.selectionChangedEventCalled = false;
		}
	}
}
