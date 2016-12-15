using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace Liberfy
{
	class ViewModelInstance : MarkupExtension
	{
		public Type InstanceType { get; set; }

		public ViewModelBase ViewModel { get; set; }

		private IProvideValueTarget _provider;
		private Window _view;

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (ViewModel != null)
			{
				InstanceType = ViewModel.GetType();
			}
			if (InstanceType != null)
			{
				var _inst = Activator.CreateInstance(InstanceType);

				if ((ViewModel = _inst as ViewModelBase) == null)
				{
					throw new NotSupportedException();
				}
			}
			else
			{
				throw new NullReferenceException();
			}

			_provider = serviceProvider
				.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

			_view = _provider.TargetObject as Window;

			if (_view != null)
			{
				registerEvents();
				ViewModel?.DialogService.RegisterView(_view);
			}

			return ViewModel;
		}

		void registerEvents()
		{
			_view.Initialized += View_Initialized;
			_view.Closing += View_Closing;
			_view.Closed += View_Closed;
		}

		void unregisterEvents()
		{
			_view.Initialized -= View_Initialized;
			_view.Closing -= View_Closing;
			_view.Closed -= View_Closed;
		}

		void View_Initialized(object sender, EventArgs e)
		{
			ViewModel?.OnInitialized();
		}

		void View_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = !ViewModel?.CanClose() ?? false;
		}

		void View_Closed(object sender, EventArgs e)
		{
			ViewModel?.Dispose();

			ViewModel?.DialogService.UnregisterView(_view);
			unregisterEvents();
		}
	}
}
