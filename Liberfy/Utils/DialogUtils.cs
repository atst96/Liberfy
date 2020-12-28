using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Liberfy.Views;

namespace Liberfy.Utils
{
    /// <summary>
    /// ウィンドウ操作に関するユーティリティクラス。
    /// </summary>
    internal static class DialogUtils
    {
        /// <summary>
        /// ウィンドウが閉じるのを待機する。
        /// </summary>
        /// <param name="window"></param>
        public static void WaitForClose(this Window window)
        {
            if (window == null)
            {
                return;
            }

            bool isClosed = false;

            void OnWindowClosed(object? sender, EventArgs args)
            {
                if (sender is Window window)
                {
                    window.Closed -= OnWindowClosed;
                }

                isClosed = true;
            }

            window.Closed += OnWindowClosed;

            while (!isClosed)
            {
                DoEvents();
            }
        }

        private static readonly Func<object, object> _doEventsCallback = args =>
        {
            if (args is DispatcherFrame frame)
            {
                frame.Continue = false;
            }

            return null;
        };

        /// <summary>
        /// DoEvents
        /// </summary>
        private static void DoEvents()
        {
            var frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, _doEventsCallback, frame);
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// 設定ウィンドウを開く。
        /// </summary>
        /// <param name="pageId">ページID</param>
        /// <param name="element">親要素</param>
        /// <param name="waitForClose">終了を待機する</param>
        public static void ShowSettingWindow(int? pageId, FrameworkElement? element, bool waitForClose = false)
        {
            var view = App.Instance.Windows
                .OfType<SettingWindow>()
                .SingleOrDefault();

            if (view != null)
            {
                view.Activate();
            }
            else
            {
                view = new SettingWindow
                {
                    Owner = element == null ? null : Window.GetWindow(element),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };

                view.Show();
            }

            if (pageId.HasValue)
            {
                view.SelectPage(pageId.Value);
            }

            if (waitForClose)
            {
                view.WaitForClose();
            }
        }
    }
}
