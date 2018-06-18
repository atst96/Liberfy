using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy.Behaviors
{
    internal class TextBoxBehavior : System.Windows.Interactivity.Behavior<TextBox>
    {
        protected override void OnDetaching()
        {
            this.Controller = null;

            base.OnDetaching();
        }

        public TextBoxController Controller
        {
            get => (TextBoxController)GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
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
            this.AssociatedObject.CaretIndex = caretIndex;
        }

        private void OnTextInserted(object sender, string text)
        {
            int startIndex = this.AssociatedObject.SelectionStart;

            this.AssociatedObject.SelectedText = text ?? "";

            this.AssociatedObject.SelectionStart = startIndex + (text?.Length ?? 0);
            this.AssociatedObject.SelectionLength = 0;
        }

        private void OnFocusHandlerCalled(object sender, EventArgs e)
        {
            this.AssociatedObject.Focus();
        }

        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller",
                typeof(TextBoxController), typeof(TextBoxBehavior),
                new PropertyMetadata(null, (obj, args) =>
                {
                    var behavior = (TextBoxBehavior)obj;

                    if (args.OldValue is TextBoxController oldController)
                    {
                        behavior.UnregisterEvents(oldController);
                    }

                    if (args.NewValue is TextBoxController newController)
                    {
                        behavior.RegisterEvents(newController);
                    }
                }));
    }
}
