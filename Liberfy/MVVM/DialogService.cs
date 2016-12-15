using Liberfy.ViewModel;
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
	class DialogService
	{
		private Application app = Application.Current;

		private Window view;
		private IntPtr hWnd;
		private ViewModelBase viewModel;
		private static Window mainView;
		private MessageBox msgBox = new MessageBox();


		public DialogService() { }

		public DialogService(ViewModelBase viewModel) : this()
		{
			this.viewModel = viewModel;
		}

		private static void SetMainView(Window view)
		{
			mainView = view;
		}

		public void RegisterView(Window view)
		{
			if (this.view != view)
			{
				UnregisterEvents();

				this.view = view;
			}

			if (view != null)
			{
				hWnd = new WindowInteropHelper(view).Handle;
				msgBox.SetWindowHandle(hWnd);

				RegisterEvents();
			}
		}

		public void UnregisterView(Window view)
		{
			if (Equals(this.view, view))
			{
				UnregisterEvents();
				msgBox.SetWindowHandle(IntPtr.Zero);
				this.view = null;
			}
		}

		private void viewLoaded(object sender, RoutedEventArgs e)
		{
			hWnd = new WindowInteropHelper(view).Handle;
			msgBox.SetWindowHandle(hWnd);
		}

		private void viewClosed(object sender, EventArgs e)
		{
			UnregisterEvents();
		}

		void RegisterEvents()
		{
			if (view != null)
			{
				view.Loaded += viewLoaded;
				view.Closed += viewClosed;
			}
		}

		void UnregisterEvents()
		{
			if (view != null)
			{
				view.Loaded -= viewLoaded;
				view.Closed -= viewClosed;
			}
		}

		public void Close(bool dialogResult)
		{
			view.DialogResult = dialogResult;
		}

		public void Invoke(ViewState viewState)
		{
			if (view == null) return;

			switch (viewState)
			{
				case ViewState.Close:
					view.Close();
					return;

				case ViewState.Minimize:
					view.WindowState = WindowState.Minimized;
					return;

				case ViewState.Maximize:
					view.WindowState = WindowState.Maximized;
					return;

				case ViewState.Normal:
					view.WindowState = WindowState.Normal;
					return;
			}
		}

		public bool OpenSetting(int? page = null, bool modal = false)
		{
			var settingWindow = app.Windows
				.OfType<SettingWindow>().SingleOrDefault();

			if (settingWindow == null)
			{
				var owner = app.MainWindow;

				settingWindow = new SettingWindow(page);

				if (owner.IsVisible)
				{
					settingWindow.Owner = owner;
				}

				if (modal) settingWindow.ShowDialog();
				else settingWindow.Show();
			}
			else
			{
				if (page.HasValue)
					settingWindow.TabPage = page.Value;

				if (settingWindow.IsVisible) settingWindow.Activate();
				else settingWindow.Show();
			}

			if(App.Accounts.Count == 0)
			{
				App.ForceExit();
				return false;
			}
			else return true;
		}

		public MsgBoxResult MessageBox(string text, MsgBoxButtons buttons = 0, MsgBoxIcon icon = 0, MsgBoxFlags flags = 0)
		{
			return msgBox.Show(text, App.ApplicationName, buttons, icon, flags);
		}

		public MsgBoxResult MessageBox(string text, string caption = null, MsgBoxButtons buttons = 0, MsgBoxIcon icon = 0, MsgBoxFlags flags = 0)
		{
			return msgBox.Show(text, caption ?? App.ApplicationName, buttons, icon, flags);
		}

		public static DialogService GetViewModel(DependencyObject obj)
		{
			return (DialogService)obj.GetValue(ViewModelProperty);
		}

		public static void SetViewModel(DependencyObject obj, DialogService value)
		{
			obj.SetValue(ViewModelProperty, value);
		}

		public static bool GetIsMain(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsMainProperty);
		}

		public static void SetIsMain(DependencyObject obj, bool value)
		{
			obj.SetValue(IsMainProperty, value);
		}

		public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.RegisterAttached("ViewModel",
				typeof(ViewModelBase), typeof(DialogService),
				new FrameworkPropertyMetadata(null, ViewModelChanged));

		public static readonly DependencyProperty IsMainProperty =
			DependencyProperty.RegisterAttached("IsMain",
				typeof(bool), typeof(ViewModelBase),
				new FrameworkPropertyMetadata(false, IsMainChanged));

		private static void ViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var window = d as Window;
			if (window != null)
			{
				var vm = e.NewValue as ViewModelBase;
				if (vm != null)
				{
					vm.DialogService.RegisterView(window);
				}
			}
		}

		private static void IsMainChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var window = d as Window;
			if (window != null)
			{
				SetMainView((bool)e.NewValue ? window : null);
			}
		}

		public void Open(ContentWindowViewModel viewModel)
		{ 
			new ContentWindow(viewModel)
			{
				Owner = view,
			}.Show();
		}

		public void Open(ContentWindowViewModel viewModel, ViewOption option)
		{
			new ContentWindow(viewModel, option)
			{
				Owner = view,
			}.Show();
		}

		public void Open(ContentWindowViewModel viewModel, ViewOption option, string templateKey)
		{
			new ContentWindow(viewModel, option, app.TryFindResource(templateKey) as DataTemplate)
			{
				Owner = view,
			}.Show();
		}

		public bool OpenModal(ContentWindowViewModel content)
		{
			return new ContentWindow(content)
			{
				Owner = view,
			}.ShowDialog() ?? false;
		}

		public bool OpenModal(ContentWindowViewModel viewModel, ViewOption option)
		{
			return new ContentWindow(viewModel, option)
			{
				Owner = view,
			}.ShowDialog() ?? false;
		}

		public bool OpenModal(ContentWindowViewModel viewModel, ViewOption option, string templateKey)
		{
			return new ContentWindow(viewModel, option, app.TryFindResource(templateKey) as DataTemplate)
			{
				Owner = view,
			}.ShowDialog() ?? false;
		}
	}

	public struct ViewOption
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
