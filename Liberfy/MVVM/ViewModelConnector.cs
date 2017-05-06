using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using Liberfy.ViewModel;

namespace Liberfy
{
	[MarkupExtensionReturnType(typeof(ViewModelBase))]
	class ViewModelConnector : MarkupExtension
	{
		public ViewModelConnector() { }

		public ViewModelConnector(Type instanceType)
		{
			_instanceType = instanceType;
		}

		public ViewModelConnector(ViewModelBase viewModel)
		{
			_viewModel = viewModel;
		}

		private Window _view;

		private Type _instanceType;
		[ConstructorArgument("instnaceType")]
		public Type InstanceType => _instanceType;

		private ViewModelBase _viewModel;
		[DefaultValue(null)]
		public ViewModelBase ViewModel => _viewModel;

		[DefaultValue(true)]
		public bool RegisterDialogService { get; set; } = true;

		[DefaultValue(false)]
		public bool IsMainView { get; set; } = false;

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (_instanceType != null)
			{
				if (Activator.CreateInstance(_instanceType) is ViewModelBase vm)
				{
					_viewModel = vm;
				}
				else
				{
					throw new NotSupportedException();
				}
			}
			else if (_viewModel != null)
			{
				_instanceType = ViewModel.GetType();
			}
			else
			{
				throw new NullReferenceException();
			}

			var dialogService = _viewModel.DialogService;
			var valueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

			if (valueTarget?.TargetObject is Window view)
			{
				_view = view;
				if (RegisterDialogService)
				{
					dialogService.RegisterView(view, IsMainView);
				}

				RegisterEvents();
			}

			return _viewModel;
		}

		void RegisterEvents()
		{
			_view.Initialized += OnViewInitialized;
			_view.Closing += OnViewClosing;
			_view.Closed += OnViewClosed;
		}

		void UnregisterEvents()
		{
			_view.Initialized -= OnViewInitialized;
			_view.Closing -= OnViewClosing;
			_view.Closed -= OnViewClosed;
		}

		void OnViewInitialized(object sender, EventArgs e)
		{
			_viewModel?.OnInitialized();
		}

		void OnViewClosing(object sender, CancelEventArgs e)
		{
			e.Cancel = !ViewModel?.CanClose() ?? false;
		}

		void OnViewClosed(object sender, EventArgs e)
		{
			var dialogService = ViewModel.DialogService;

			if (_viewModel != null)
			{
				_viewModel.OnClosed();
				_viewModel.Dispose();
			}

			UnregisterEvents();

			_view.DataContext = null;

			_instanceType = null;
			_viewModel = null;
			_view = null;
		}
	}
}
