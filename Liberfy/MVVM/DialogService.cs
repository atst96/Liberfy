using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Liberfy
{
	class DialogService
	{
		private Application app = Application.Current;

		private Window view;
		private ViewModelBase viewModel;
		private static Window mainView;

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
				RegisterEvents();
			}
		}

		public void UnregisterView(Window view)
		{
			if(Equals(this.view, view))
			{
				UnregisterEvents();
				this.view = null;
			}
		}

		private void ViewClosing(object sender, CancelEventArgs e)
		{
			e.Cancel = !viewModel?.CanClose() ?? true;
		}

		private void ViewClosed(object sender, EventArgs e)
		{
			UnregisterEvents();
		}

		void RegisterEvents()
		{
			if (view != null)
			{
				view.Closed += ViewClosed;
				view.Closing += ViewClosing;
			}
		}

		void UnregisterEvents()
		{
			if (view != null)
			{
				view.Closed -= ViewClosed;
				view.Closing -= ViewClosing;
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
					if (view.IsVisible)
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

		public void OpenSetting(int? page = null, bool modal = false)
		{
			var settingWindow = app.Windows
				.OfType<SettingWindow>().SingleOrDefault();

			if (settingWindow == null)
			{
				var owner = app.MainWindow;

				settingWindow = new SettingWindow(page);

				if(owner.IsVisible)
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
	}

	public enum ViewState
	{
		Close,
		Minimize,
		Maximize,
		Normal,
	}
}
