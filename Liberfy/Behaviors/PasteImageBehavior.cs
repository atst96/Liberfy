using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Liberfy.Behaviors
{
    internal class PasteImageBehavior : Microsoft.Xaml.Behaviors.Behavior<FrameworkElement>, ICommandSource
    {
        public object CommandParameter => throw new NotImplementedException();

        public IInputElement CommandTarget => throw new NotImplementedException();

        private CommandBinding _commandBinding;

        protected override void OnAttached()
        {
            this._commandBinding = new CommandBinding(
                ApplicationCommands.Paste,
                this.OnPasteCommandExecuted,
                this.CanPasteCommandExecute);

            this.AssociatedObject.CommandBindings.Add(this._commandBinding);

            base.OnAttached();
        }

        private void CanPasteCommandExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.Command?.CanExecute(e.Parameter) ?? false)
            {
                e.CanExecute = true;
                e.Handled = true;
            }
        }

        private void OnPasteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Command?.Execute(e.Parameter);
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.CommandBindings.Remove(this._commandBinding);
            this._commandBinding.Executed -= this.OnPasteCommandExecuted;
            this._commandBinding.CanExecute -= this.CanPasteCommandExecute;
            this._commandBinding = null;

            base.OnDetaching();
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command",
                typeof(ICommand), typeof(PasteImageBehavior),
                new PropertyMetadata(null));
    }
}
