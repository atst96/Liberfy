using System.Linq;
using System.Windows;
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
            var view = App.Instance.Windows
                .OfType<Views.SettingWindow>()
                .SingleOrDefault();

            if (view != null)
            {
                view.Activate();
            }
            else
            {
                view = new Views.SettingWindow
                {
                    Owner = Window.GetWindow(this.AssociatedObject),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };
                view.Show();
            }

            var actionMessage = message as OpenSettingDialogMessage;

            if (actionMessage == null)
            {
                return;
            }

            if (actionMessage.TabPageIndex.HasValue)
            {
                (view.DataContext as ViewModels.SettingWindowViewModel).TabPageIndex = actionMessage.TabPageIndex.Value;
            }
        }
    }
}
