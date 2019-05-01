using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using Liberfy.ViewModels;

namespace Liberfy
{
    [MarkupExtensionReturnType(typeof(ViewModelBase))]
    internal class ViewModelConnector : MarkupExtension
    {
        public ViewModelConnector() { }

        public ViewModelConnector(Type instanceType)
        {
            this.InstanceType = instanceType;
        }

        public ViewModelConnector(ViewModelBase viewModel)
        {
            this.ViewModel = viewModel;
        }

        private Window _view;

        [ConstructorArgument("instnaceType")]
        public Type InstanceType { get; private set; }

        [DefaultValue(null)]
        public ViewModelBase ViewModel { get; private set; }

        [DefaultValue(true)]
        public bool RegisterDialogService { get; set; } = true;

        [DefaultValue(true)]
        public bool RegisterWindowService { get; set; } = true;

        [DefaultValue(false)]
        public bool IsMainView { get; set; } = false;

        private static readonly Type _providerType = typeof(IProvideValueTarget);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.InstanceType != null)
            {
                this.ViewModel = Activator.CreateInstance(this.InstanceType) as ViewModelBase ?? throw new NotSupportedException();
            }
            else if (this.ViewModel != null)
            {
                this.InstanceType = this.ViewModel.GetType();
            }
            else
            {
                throw new NullReferenceException();
            }

            var valueTargetService = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (valueTargetService?.TargetObject is Window view)
            {
                var dialogService = this.ViewModel.DialogService;
                var windowService = this.ViewModel.WindowService;

                this._view = view;

                if (this.RegisterDialogService)
                {
                    dialogService.RegisterView(view, this.IsMainView);
                }

                if (this.RegisterWindowService)
                {
                    windowService.SetView(view, this.IsMainView);
                }

                this.RegisterEvents();
            }

            return this.ViewModel;
        }

        private void RegisterEvents()
        {
            this._view.Initialized += this.OnViewInitialized;
            this._view.Closing += this.OnViewClosing;
            this._view.Closed += this.OnViewClosed;
        }

        private void UnregisterEvents()
        {
            this._view.Initialized -= this.OnViewInitialized;
            this._view.Closing -= this.OnViewClosing;
            this._view.Closed -= this.OnViewClosed;
        }

        private void OnViewInitialized(object sender, EventArgs e)
        {
            this.ViewModel?.OnInitialized();
        }

        private void OnViewClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = !(this.ViewModel?.CanClose() ?? false) || e.Cancel;
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.OnClosed();
                this.ViewModel.Dispose();
            }

            this.UnregisterEvents();

            this._view.DataContext = null;

            this.InstanceType = null;
            this.ViewModel = null;
            this._view = null;
        }
    }
}
