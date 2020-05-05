using System;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy.Behaviors
{
    /// <summary>
    /// TextBox操作のビヘイビア
    /// </summary>
    internal class TextBoxBehavior : Microsoft.Xaml.Behaviors.Behavior<TextBox>
    {
        /// <summary>
        /// ビヘイビアのデタッチ時
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.Controller = null;
        }

        /// <summary>
        /// Controllerのプロパティ
        /// </summary>
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register(nameof(Controller),
                typeof(TextBoxController), typeof(TextBoxBehavior), new PropertyMetadata(null, OnControllerPropertyChanged));

        /// <summary>
        /// TextBoxのコントローラを取得または設定する。
        /// </summary>
        public TextBoxController Controller
        {
            get => (TextBoxController)this.GetValue(ControllerProperty);
            set => this.SetValue(ControllerProperty, value);
        }

        private void RegisterEvents(TextBoxController controller)
        {
            controller.InsertHandler += this.OnTextInserted;
            controller.SetCaretIndexHandler += this.OnCaretIndexSetted;
            controller.FocusHandler += this.OnFocusHandlerCalled;
        }

        private void UnregisterEvents(TextBoxController controller)
        {
            controller.InsertHandler -= this.OnTextInserted;
            controller.SetCaretIndexHandler -= this.OnCaretIndexSetted;
            controller.FocusHandler -= this.OnFocusHandlerCalled;
        }

        private void OnCaretIndexSetted(object sender, int caretIndex)
        {
            var textBox = this.AssociatedObject;
            textBox.CaretIndex = caretIndex;
        }

        private void OnTextInserted(object sender, string text)
        {
            int startIndex = this.AssociatedObject.SelectionStart;

            var textBox = this.AssociatedObject;
            textBox.SelectedText = text ?? "";

            textBox.SelectionStart = startIndex + (text?.Length ?? 0);
            textBox.SelectionLength = 0;
        }

        private void OnFocusHandlerCalled(object sender, EventArgs e)
        {
            var textBox = this.AssociatedObject;
            textBox.Focus();
        }

        /// <summary>
        /// Controllerプロパティ変更時
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnControllerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (TextBoxBehavior)d;

            if (e.OldValue is TextBoxController oldController)
            {
                behavior.UnregisterEvents(oldController);
            }

            if (e.NewValue is TextBoxController newController)
            {
                behavior.RegisterEvents(newController);
            }
        }
    }
}
