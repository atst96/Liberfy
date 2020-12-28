using System.Windows;
using Liberfy.Utils;
using Livet.Behaviors.Messaging;
using Livet.Messaging;

namespace Liberfy.Messaging
{
    /// <summary>
    /// 設定ダイアログを表示するアクション。<see cref="OpenSettingDialogMessage"/>に対応する。
    /// </summary>
    internal class OpenSettingViewDialogAction : InteractionMessageAction<FrameworkElement>
    {
        /// <summary>
        /// <see cref="OpenSettingViewDialogAction"/>を生成する。
        /// </summary>
        public OpenSettingViewDialogAction() : base()
        {
        }

        /// <summary>
        /// アクションを実行する。
        /// </summary>
        /// <param name="message"></param>
        protected override void InvokeAction(InteractionMessage message)
        {
            if (message is OpenSettingDialogMessage actionMessage)
            {
                DialogUtils.ShowSettingWindow(actionMessage.TabPageIndex, this.AssociatedObject, false);
            }
        }
    }
}
