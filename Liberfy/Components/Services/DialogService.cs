using Liberfy.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shell;

namespace Liberfy.Components
{
    internal class DialogService : IDisposable
    {
        private readonly Application app = Application.Current;

        private Window _view;
        private IntPtr _hWnd;
        private ViewModelBase _viewModel;
        private static Window mainView;


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
                this._view = null;
            }
        }

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            this._hWnd = new WindowInteropHelper(_view).Handle;
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

        public static TaskDialogResult ShowTaskDialog(IntPtr hWnd, string message, string caption, string instruction, TaskDialogStandardButtons buttons, TaskDialogStandardIcon icon)
        {
            var taskDialog = new TaskDialog()
            {
                OwnerWindowHandle = hWnd,
                Text = message,
                Caption = caption ?? App.Name,
                InstructionText = instruction,
                StandardButtons = buttons,
                Icon = icon,
                StartupLocation = TaskDialogStartupLocation.CenterOwner,
            };

            using (taskDialog)
            {
                return taskDialog.Show();
            }
        }

        public void WarningMessage(string message, string instruction = null, string caption = null)
        {
            ShowTaskDialog(this._hWnd, message, caption, instruction, TaskDialogStandardButtons.Close, TaskDialogStandardIcon.Warning);
        }

        public void ErrorMessage(string message, string instruction = null, string caption = null)
        {
            ShowTaskDialog(this._hWnd, message, caption, instruction, TaskDialogStandardButtons.Close, TaskDialogStandardIcon.Error);
        }

        public void InformationMessage(string message, string instruciton = null, string caption = null)
        {
            ShowTaskDialog(this._hWnd, message, caption, instruciton, TaskDialogStandardButtons.Close, TaskDialogStandardIcon.Information);
        }

        public bool Confirm(string message, string instruction = null, string caption = null)
        {
            return ShowTaskDialog(this._hWnd, message, caption, instruction, TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No, TaskDialogStandardIcon.Information) == TaskDialogResult.Yes;
        }

        private static bool ShowCommonFileDialog(IntPtr hWnd, CommonFileDialog fileDialog)
        {
            return fileDialog.ShowDialog(hWnd) == CommonFileDialogResult.Ok;
        }

        private static void SetCmmonFileDialogFilter(CommonFileDialog fileDialog, IDictionary<string, string[]> filters)
        {
            foreach (var kvp in filters)
            {
                var filterText = string.Join(";", kvp.Value.Select(ext => "*" + ext));

                fileDialog.Filters.Add(new CommonFileDialogFilter(kvp.Key, filterText));
            }
        }

        public string SelectOpenFile(string title, IDictionary<string, string[]> filters)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = title,
                Multiselect = false,
            };

            SetCmmonFileDialogFilter(dialog, filters);

            return ShowCommonFileDialog(this._hWnd, dialog) ? dialog.FileName : null;
        }

        public IEnumerable<string> SelectOpenFiles(string title, IDictionary<string, string[]> filters)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = title,
                Multiselect = true,
            };

            SetCmmonFileDialogFilter(dialog, filters);

            return ShowCommonFileDialog(this._hWnd, dialog) ? dialog.FileNames : null;
        }

        public string SelectSaveFile(string title, IDictionary<string, string[]> filters)
        {
            var dialog = new CommonSaveFileDialog
            {
                Title = title,
            };

            SetCmmonFileDialogFilter(dialog, filters);

            return ShowCommonFileDialog(this._hWnd, dialog) ? dialog.FileName : null;
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
