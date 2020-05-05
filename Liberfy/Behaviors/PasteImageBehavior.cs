using System;
using System.Windows;
using System.Windows.Input;

namespace Liberfy.Behaviors
{
    /// <summary>
    /// ペースト操作用ビヘイビア
    /// </summary>
    internal class PasteImageBehavior : Microsoft.Xaml.Behaviors.Behavior<FrameworkElement>, ICommandSource
    {
        [Obsolete]
        public object CommandParameter => throw new NotImplementedException();

        [Obsolete]
        public IInputElement CommandTarget => throw new NotImplementedException();

        /// <summary>
        /// ペースト操作用のコマンドバインディング
        /// </summary>
        private CommandBinding _commandBinding;


        /// <summary>
        /// ビヘイビアのアタッチ時
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            // コマンドバインディングを生成してAssociatedObjectに登録
            this._commandBinding = new CommandBinding(ApplicationCommands.Paste, this.OnPasteCommandExecuted, this.CanPasteCommandExecute);

            this.AssociatedObject.CommandBindings.Add(this._commandBinding);
        }

        /// <summary>
        /// ビヘイビアのデタッチ時
        /// </summary>
        protected override void OnDetaching()
        {
            // コマンドバインディングを破棄
            this.AssociatedObject.CommandBindings.Remove(this._commandBinding);

            this._commandBinding.Executed -= this.OnPasteCommandExecuted;
            this._commandBinding.CanExecute -= this.CanPasteCommandExecute;
            this._commandBinding = null;

            base.OnDetaching();
        }

        /// <summary>
        /// ペースト操作が可能かどうかをコマンドに問い合わせる。
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e"><see cref="CanExecuteRoutedEventArgs"/></param>
        private void CanPasteCommandExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.Command?.CanExecute(e.Parameter) ?? false)
            {
                e.CanExecute = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// コマンドを実行する。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"><see cref="ExecutedRoutedEventArgs"/></param>
        private void OnPasteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Command?.Execute(e.Parameter);
        }

        /// <summary>
        /// コマンドのプロパティ
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(PasteImageBehavior), new PropertyMetadata(null));

        /// <summary>
        /// コマンドを取得または設定する。
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)this.GetValue(CommandProperty);
            set => this.SetValue(CommandProperty, value);
        }
    }
}
