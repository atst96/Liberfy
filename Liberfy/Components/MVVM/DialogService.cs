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
        private readonly MessageBox msgBox = new MessageBox();


        public DialogService() { }

        public DialogService(ViewModelBase viewModel) : this()
        {
            this._viewModel = viewModel;
        }

        internal void RegisterView(Window view, bool isMainView = false)
        {
            if (!object.Equals(this._view, view))
            {
                this.UnregisterView(view);

                this._view = view;
            }

            if (view != null)
            {
                this._hWnd = new WindowInteropHelper(view).Handle;
                this.msgBox.SetWindowHandle(_hWnd);

                this.RegisterEvents();

                if (isMainView)
                {
                    mainView = view;
                }
            }
        }

        internal void UnregisterView(Window view)
        {
            if (object.Equals(this._view, view))
            {
                this.UnregisterEvents();
                this.msgBox.Dispose();
                this._view = null;
            }
        }

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            this._hWnd = new WindowInteropHelper(_view).Handle;
            this.msgBox.SetWindowHandle(_hWnd);
        }

        private void ViewClosed(object sender, EventArgs e)
        {
            this.UnregisterView(_view);
        }

        private void RegisterEvents()
        {
            if (this._view != null)
            {
                this._view.Loaded += ViewLoaded;
                this._view.Closed += ViewClosed;
            }
        }

        private void UnregisterEvents()
        {
            if (_view != null)
            {
                this._view.Loaded -= ViewLoaded;
                this._view.Closed -= ViewClosed;
            }
        }

        public void Close(bool dialogResult)
        {
            this._view.DialogResult = dialogResult;
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

        public MsgBoxResult MessageBox(string text, MsgBoxButtons buttons = 0, MsgBoxIcon icon = 0, MsgBoxFlags flags = 0)
        {
            return msgBox.Show(text, App.AppName, buttons, icon, flags);
        }

        public MsgBoxResult MessageBox(string text, string caption = null, MsgBoxButtons buttons = 0, MsgBoxIcon icon = 0, MsgBoxFlags flags = 0)
        {
            return msgBox.Show(text, caption ?? App.AppName, buttons, icon, flags);
        }

        private bool ShowFileDialog(FileDialog dialog)
        {
            return dialog.ShowDialog(_view) ?? false;
        }

        public string SelectOpenFile(string title, string filter)
        {
            var ofd = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                Multiselect = false,
            };

            return ShowFileDialog(ofd) ? ofd.FileName : null;
        }

        public string[] SelectOpenFiles(string title, string filter)
        {
            var ofd = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                Multiselect = true,
            };

            return ShowFileDialog(ofd) ? ofd.FileNames : null;
        }

        public string SelectSaveFile(string title, string filter)
        {
            var ofd = new SaveFileDialog
            {
                Title = title,
                Filter = filter,
            };

            return ShowFileDialog(ofd) ? ofd.FileName : null;
        }

        public bool ShowQuestion(string content)
        {
            var result = MessageBox(
                content, App.AppName,
                MsgBoxButtons.YesNo, MsgBoxIcon.Question);

            return result == MsgBoxResult.Yes;
        }

        public void Dispose()
        {
            if (mainView == _view)
            {
                mainView = null;
            }

            this._hWnd = IntPtr.Zero;
            this._view = null;
            this._viewModel = null;
        }
    }

    public enum ViewState
    {
        Close,
        Minimize,
        Maximize,
        Normal,
    }
}
