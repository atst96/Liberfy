using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Liberfy.Behaviors
{
	internal class PasteImageBehavior : Behavior<FrameworkElement>, ICommandSource
	{
		public object CommandParameter => throw new NotImplementedException();

		public IInputElement CommandTarget => throw new NotImplementedException();

		CommandBinding commandBinding;

		protected override void OnAttached()
		{
			commandBinding = new CommandBinding(
				ApplicationCommands.Paste,
				OnPasteCommandExecuted,
				CanPasteCommandExecute);

			AssociatedObject.CommandBindings.Add(commandBinding);

			base.OnAttached();
		}

		private void CanPasteCommandExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if(Command?.CanExecute(e.Parameter) ?? false)
			{
				e.CanExecute = true;
				e.Handled = true;
			}
		}

		private void OnPasteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Command?.Execute(e.Parameter);
		}

		protected override void OnDetaching()
		{
			AssociatedObject.CommandBindings.Remove(commandBinding);
			commandBinding.Executed -= OnPasteCommandExecuted;
			commandBinding.CanExecute -= CanPasteCommandExecute;
			commandBinding = null;

			base.OnDetaching();
		}

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(PasteImageBehavior), new PropertyMetadata(null));
	}
}
