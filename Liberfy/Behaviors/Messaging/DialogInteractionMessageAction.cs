using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Liberfy.Messaging;
using Livet.Behaviors.Messaging;
using Livet.Messaging;

namespace Liberfy.Behaviors.Messaging
{
    internal class DialogInteractionMessageAction : InteractionMessageAction<FrameworkElement>
    {
        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        private IntPtr _hwnd;

        protected override void OnAttached()
        {
            base.OnAttached();

            if (this.AssociatedObject.IsLoaded)
            {
                this.Initialize();
            }
            else
            {
                this.AssociatedObject.Loaded += this.OnLoaded;
            }
        }

        /// <summary>
        /// Loadedイベント発行時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.Loaded -= this.OnLoaded;
            this.Initialize();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            var window = Window.GetWindow(this.AssociatedObject);
            this._hwnd = new WindowInteropHelper(window).Handle;
        }

        /// <summary>
        /// アクション実行
        /// </summary>
        /// <param name="message"></param>
        protected override void InvokeAction(InteractionMessage message)
        {
            switch (message)
            {
                case InformationMessage infoMessage:
                    this.InformationMessage(infoMessage);
                    return;

                case InformationDialogMessage infoDialogMessage:
                    this.InformationDialogMessage(infoDialogMessage);
                    break;
            }
        }

        /// <summary>
        /// ダイアログ表示
        /// </summary>
        /// <param name="message"></param>
        private void InformationMessage(InformationMessage message)
        {
            _ = TaskDialog.ShowDialog(this._hwnd, new()
            {
                Caption = message.Caption,
                Text = message.Text,
                Icon = GetDialogIcon(message.Image),
            });
        }

        /// <summary>
        /// <see cref="MessageBoxImage"/>から<see cref="TaskDialogIcon"/>を取得する
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static TaskDialogIcon GetDialogIcon(MessageBoxImage image) => image switch
        {
            MessageBoxImage.None => TaskDialogIcon.None,
            MessageBoxImage.Error => TaskDialogIcon.Error,
            MessageBoxImage.Question => TaskDialogIcon.Information,
            MessageBoxImage.Exclamation => TaskDialogIcon.Warning,
            MessageBoxImage.Asterisk => TaskDialogIcon.Error,
            _ => TaskDialogIcon.None,
        };

        /// <summary>
        /// ダイアログ表示
        /// </summary>
        /// <param name="message"></param>
        private void InformationDialogMessage(InformationDialogMessage message)
        {
            var footnote = message.FootNoteText != null ? null : new TaskDialogFootnote
            {
                Text = message.FootNoteText,
                Icon = message.FootNoteIcon,
            };

            var expander = message.ExpanderText == null ? null : new TaskDialogExpander
            {
                Text = message.ExpanderText,
                Expanded = message.IsExpanded,
                Position = message.ExpanderPosition,
            };

            _ = TaskDialog.ShowDialog(this._hwnd, new()
            {
                Caption = message.Caption,
                Text = message.Text,
                Icon = message.Icon,
                Heading = message.Heading,
                AllowCancel = message.AllowCancel,
                AllowMinimize = message.AllowMimimize,
                Expander = expander,
                Footnote = footnote,
            });
        }
    }
}
