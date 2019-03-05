using Liberfy.ViewModels;
using Liberfy.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Liberfy.Components
{
    internal class WindowService
    {
        private readonly static Application App = Application.Current;

        private Window _view;
        private readonly ViewModelBase _viewModel;
        private static Window _mainView;

        public WindowService() { }

        public WindowService(ViewModelBase viewModel) : this()
        {
            this._viewModel = viewModel;
        }

        internal void SetView(Window view, bool isMainWindow)
        {
            if (!object.Equals(this._view, view))
            {
                this._view = view;
            }

            if (view != null && isMainWindow)
            {
                _mainView = view;
            }
        }

        public void OpenSetting(int? pageIndex = default, bool isModal = false)
        {
            var window = App.Windows
                .OfType<SettingWindow>().SingleOrDefault() ?? new SettingWindow();

            if (pageIndex.HasValue)
            {
                window.MoveTabPage(pageIndex.Value);
            }

            if (window.IsVisible)
            {
                window.Activate();
            }
            else
            {
                if (App.MainWindow.IsLoaded)
                {
                    window.Owner = App.MainWindow;
                }

                if (isModal)
                {
                    window.ShowDialog();
                }
                else
                {
                    window.Show();
                }
            }
        }

        public void OpenTweetWindow()
        {
            new TweetWindow().Show();
        }

        public void OpenTweetWindow(IAccount account)
        {
            new TweetWindow(account).Show();
        }

        public void OpenAuthenticationWindow()
        {
            new AccountAuthenticationWindow()
            {
                Owner = this._view,
            }.ShowDialog();
        }
    }
}
