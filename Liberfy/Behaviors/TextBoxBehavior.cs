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
	internal class TextBoxBehavior : Behavior<TextBox>
	{
		protected override void OnDetaching()
		{
			Controller = null;

			base.OnDetaching();
		}

		public TextBoxController Controller
		{
			get { return (TextBoxController)GetValue(ControllerProperty); }
			set { SetValue(ControllerProperty, value); }
		}

		private void RegisterEvents(TextBoxController controller)
		{
			controller.InsertHandler += OnTextInserted;
			controller.SetCaretIndexHandler += OnCaretIndexSetted;
			controller.FocusHandler += OnFocusHandlerCalled;
		}

		private void UnregisterEvents(TextBoxController controller)
		{
			controller.InsertHandler -= OnTextInserted;
			controller.SetCaretIndexHandler -= OnCaretIndexSetted;
			controller.FocusHandler -= OnFocusHandlerCalled;
		}

		private void OnCaretIndexSetted(object sender, int caretIndex)
		{
			AssociatedObject.CaretIndex = caretIndex;
		}

		private void OnTextInserted(object sender, string text)
		{
			int startIndex = AssociatedObject.SelectionStart;

			AssociatedObject.SelectedText = text;

			AssociatedObject.SelectionStart = startIndex + text.Length;
			AssociatedObject.SelectionLength = 0;
		}

		private void OnFocusHandlerCalled(object sender, EventArgs e)
		{
			AssociatedObject.Focus();
		}

		public static readonly DependencyProperty ControllerProperty
			= DependencyProperty.Register("Controller",
				typeof(TextBoxController), typeof(TextBoxBehavior),
				new PropertyMetadata(null, ControllerChanged));

		private static void ControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var tbb = (TextBoxBehavior)d;

			if(e.OldValue is TextBoxController oldController)
			{
				tbb.UnregisterEvents(oldController);
			}

			if(e.NewValue is TextBoxController newController)
			{
				tbb.RegisterEvents(newController);
			}
		}
	}
}
