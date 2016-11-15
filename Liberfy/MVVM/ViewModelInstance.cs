using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace Liberfy
{
	class ViewModelInstance : MarkupExtension
	{
		public Type InstanceType { get; set; }

		public ViewModelBase ViewModel
		{
			get { return _viewModel; }
			set { _viewModel = value; }
		}

		private IProvideValueTarget _provider;
		private ViewModelBase _viewModel;
		private Window _view;

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (_viewModel != null)
			{
				InstanceType = _viewModel.GetType();
			}
			if (InstanceType != null)
			{
				var _inst = Activator.CreateInstance(InstanceType);

				if ((_viewModel = _inst as ViewModelBase) == null)
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
				_viewModel?.DialogService.RegisterView(_view);
			}

			return _viewModel;
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
			_viewModel?.OnInitialized();
		}

		void View_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = !_viewModel?.CanClose() ?? false;
		}

		void View_Closed(object sender, EventArgs e)
		{
			_viewModel?.Dispose();

			_viewModel?.DialogService.UnregisterView(_view);
			unregisterEvents();
		}
	}
}
