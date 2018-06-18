using Liberfy.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shell;

namespace Liberfy
{
    class DialogService : IDisposable
    {
        private Application app = Application.Current;

        private Window _view;
        private IntPtr _hWnd;
        private ViewModelBase _viewModel;
        private static Window mainView;
        private MessageBox msgBox = new MessageBox();


        public DialogService() { }

        public DialogService(ViewModelBase viewModel) : this()
        {
            this._viewModel = viewModel;
        }

        internal void RegisterView(Window view, bool isMainView = false)
        {
            if (!Equals(this._view, view))
            {
                UnregisterView(view);

                this._view = view;
            }

            if (view != null)
            {
                _hWnd = new WindowInteropHelper(view).Handle;
                msgBox.SetWindowHandle(_hWnd);

                RegisterEvents();

                if (isMainView)
                {
                    mainView = view;
                }
            }
        }

        internal void UnregisterView(Window view)
        {
            if (Equals(this._view, view))
            {
                UnregisterEvents();
                msgBox.Dispose();
                this._view = null;
            }
        }

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            _hWnd = new WindowInteropHelper(_view).Handle;
            msgBox.SetWindowHandle(_hWnd);
        }

        private void ViewClosed(object sender, EventArgs e)
        {
            UnregisterView(_view);
        }

        private void RegisterEvents()
        {
            if (_view != null)
            {
                _view.Loaded += ViewLoaded;
                _view.Closed += ViewClosed;
            }
        }

        private void UnregisterEvents()
        {
            if (_view != null)
            {
                _view.Loaded -= ViewLoaded;
                _view.Closed -= ViewClosed;
            }
        }

        public void Close(bool dialogResult)
        {
            _view.DialogResult = dialogResult;
        }

        public void Open(ViewType viewType, object parameter = null)
        {
            ShowView(viewType, _view, false, parameter);
        }

        public bool OpenModal(ViewType viewType, object parameter = null)
        {
            return ShowView(viewType, _view, true, parameter);
        }

        public static void OpenWithMainView(ViewType viewType, object parameter = null)
        {
            ShowView(viewType, mainView, false, parameter);
        }

        public static bool OpenWithMainViewModal(ViewType viewType, object parameter = null)
        {
            return ShowView(viewType, mainView, true, parameter);
        }

        private static bool ShowView(ViewType viewType, Window owner, bool isModal, object parameter = null)
        {
            var w = WindowFromViewType(viewType, parameter);
            w.Owner = owner;

            if (isModal)
            {
                return w.ShowDialog() ?? false;
            }
            else
            {
                w.Show();
                return false;
            }
        }

        private static Window WindowFromViewType(ViewType viewType, object parameter = null)
        {
            switch (viewType)
            {
                case ViewType.TweetWindow:
                    return new View.TweetWindow(parameter);

                default:
                    throw new NotSupportedException();
            }
        }

        public void Invoke(ViewState viewState)
        {
            if (_view == null) return;

            switch (viewState)
            {
                case ViewState.Close:
                    _view.Close();
                    return;

                case ViewState.Minimize:
                    _view.WindowState = System.Windows.WindowState.Minimized;
                    return;

                case ViewState.Maximize:
                    _view.WindowState = System.Windows.WindowState.Maximized;
                    return;

                case ViewState.Normal:
                    _view.WindowState = System.Windows.WindowState.Normal;
                    return;
            }
        }

        public bool OpenInitSettingView()
        {
            OpenSetting(0, true);
            return App.Accounts.Count > 0;
        }

        public void OpenSetting(int? page = null, bool isModal = false)
        {
            var settingWindow = app.Windows
                .OfType<SettingWindow>()
                .SingleOrDefault() ?? new SettingWindow();

            if (page.HasValue)
            {
                settingWindow.MoveTabPage(page.Value);
            }

            if (settingWindow.IsVisible)
            {
                settingWindow.Activate();
            }
            else
            {
                if (app.MainWindow.IsLoaded)
                    settingWindow.Owner = app.MainWindow;

                if (isModal)
                    settingWindow.ShowDialog();
                else
                    settingWindow.Show();
            }
        }

        public MsgBoxResult MessageBox(string text, MsgBoxButtons buttons = 0, MsgBoxIcon icon = 0, MsgBoxFlags flags = 0)
        {
            return msgBox.Show(text, App.AppName, buttons, icon, flags);
        }

        public MsgBoxResult MessageBox(string text, string caption = null, MsgBoxButtons buttons = 0, MsgBoxIcon icon = 0, MsgBoxFlags flags = 0)
        {
            return msgBox.Show(text, caption ?? App.AppName, buttons, icon, flags);
        }

        public void Open(ContentWindowViewModel viewModel)
        {
            new ContentWindow(viewModel)
            {
                Owner = _view,
            }.Show();
        }

        public void Open(ContentWindowViewModel viewModel, ViewOption option)
        {
            new ContentWindow(viewModel, option)
            {
                Owner = _view,
            }.Show();
        }

        public void Open(ContentWindowViewModel viewModel, ViewOption option, string templateKey)
        {
            new ContentWindow(viewModel, option, app.TryFindResource(templateKey) as DataTemplate)
            {
                Owner = _view,
            }.Show();
        }

        public bool OpenModal(ContentWindowViewModel content)
        {
            return new ContentWindow(content)
            {
                Owner = _view,
            }.ShowDialog() ?? false;
        }

        public bool OpenModal(ContentWindowViewModel viewModel, ViewOption option)
        {
            return new ContentWindow(viewModel, option)
            {
                Owner = _view,
            }.ShowDialog() ?? false;
        }

        public bool OpenModal(ContentWindowViewModel viewModel, ViewOption option, string templateKey)
        {
            return new ContentWindow(viewModel, option, app.TryFindResource(templateKey) as DataTemplate)
            {
                Owner = _view,
            }.ShowDialog() ?? false;
        }

        public bool Open(OpenFileDialog ofd)
        {
            return ofd?.ShowDialog() ?? false;
        }

        public bool OpenModal(OpenFileDialog ofd)
        {
            return ofd?.ShowDialog(_view) ?? false;
        }

        public bool ShowQuestion(string content)
        {
            var result = MessageBox(
                content, App.AppName,
                MsgBoxButtons.YesNo, MsgBoxIcon.Question);

            return result == MsgBoxResult.Yes;
        }

        public SelectDialogOption<T> SelectDialog<T>(IEnumerable<T> items)
        {
            return this.SelectDialog(new SelectDialogOption<T>() { Items = items, });
        }

        public SelectDialogOption<T> SelectDialog<T>(IEnumerable<T> items, string instruction)
        {
            return this.SelectDialog(new SelectDialogOption<T>()
            {
                Instruction = instruction,
                Items = items,
            });
        }

        public SelectDialogOption<T> SelectDialog<T>(SelectDialogOption<T> option)
        {
            var v = new View.SelectItemWindow()
            {
                DataContext = option,
                Owner = _view,
            };

            option.IsSelected = v.ShowDialog() ?? false;

            return option;
        }

        public void Dispose()
        {
            if (mainView == _view)
            {
                mainView = null;
            }

            _hWnd = IntPtr.Zero;
            _view = null;
            _viewModel = null;
        }
    }

    public enum ViewType
    {
        TweetWindow,
    }

    internal struct ViewOption
    {
        public double? Width { get; set; }
        public double? Height { get; set; }
        public ResizeMode? ResizeMode { get; set; }
        public SizeToContent? SizeToContent { get; set; }
        public WindowChrome WindowChrome { get; set; }
        public WindowStyle? Style { get; set; }
        public WindowState? State { get; set; }
        public WindowStartupLocation? StartupLocation { get; set; }
        public bool? ShowInTaskbar { get; set; }
    }

    public enum ViewState
    {
        Close,
        Minimize,
        Maximize,
        Normal,
    }
}
