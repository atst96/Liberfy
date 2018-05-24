using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using Liberfy.ViewModel;

namespace Liberfy
{
    [MarkupExtensionReturnType(typeof(ViewModelBase))]
    internal class ViewModelConnector : MarkupExtension
    {
        public ViewModelConnector() { }

        public ViewModelConnector(Type instanceType)
        {
            this._instanceType = instanceType;
        }

        public ViewModelConnector(ViewModelBase viewModel)
        {
            this._viewModel = viewModel;
        }

        private Window _view;

        private Type _instanceType;
        [ConstructorArgument("instnaceType")]
        public Type InstanceType => _instanceType;

        private ViewModelBase _viewModel;
        [DefaultValue(null)]
        public ViewModelBase ViewModel => this._viewModel;

        [DefaultValue(true)]
        public bool RegisterDialogService { get; set; } = true;

        [DefaultValue(false)]
        public bool IsMainView { get; set; } = false;

        private static Type _providerType = typeof(IProvideValueTarget);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this._instanceType != null)
            {
                this._viewModel = Activator.CreateInstance(this._instanceType) as ViewModelBase
                    ?? throw new NotSupportedException();
            }
            else if (this._viewModel != null)
            {
                this._instanceType = this.ViewModel.GetType();
            }
            else
            {
                throw new NullReferenceException();
            }

            var dialogService = this._viewModel.DialogService;

            if (serviceProvider.GetService(_providerType) is IProvideValueTarget valueTarget
                && valueTarget.TargetObject is Window view)
            {
                this._view = view;
                if (this.RegisterDialogService)
                {
                    dialogService.RegisterView(view, IsMainView);
                }

                this.RegisterEvents();
            }

            return _viewModel;
        }

        void RegisterEvents()
        {
            this._view.Initialized += this.OnViewInitialized;
            this._view.Closing += this.OnViewClosing;
            this._view.Closed += this.OnViewClosed;
        }

        void UnregisterEvents()
        {
            this._view.Initialized -= this.OnViewInitialized;
            this._view.Closing -= this.OnViewClosing;
            this._view.Closed -= this.OnViewClosed;
        }

        void OnViewInitialized(object sender, EventArgs e)
        {
            this._viewModel?.OnInitialized();
        }

        void OnViewClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = !this.ViewModel?.CanClose() ?? false;
        }

        void OnViewClosed(object sender, EventArgs e)
        {
            var dialogService = this.ViewModel.DialogService;

            if (this._viewModel != null)
            {
                this._viewModel.OnClosed();
                this._viewModel.Dispose();
            }

            this.UnregisterEvents();

            this._view.DataContext = null;

            this._instanceType = null;
            this._viewModel = null;
            this._view = null;
        }
    }
}
