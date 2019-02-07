using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Liberfy.ViewModel;
using System.Windows.Shell;
using System.ComponentModel;
using System.Windows.Markup;

namespace Liberfy
{
    /// <summary>
    /// ContentWindow.xaml の相互作用ロジック
    /// </summary>
    partial class ContentWindow : Window
    {
        public ContentWindow()
        {
            this.InitializeComponent();
        }

        private void SetDataContext(ContentWindowViewModel viewModel)
        {
            this.DataContext = new ViewModelConnector(viewModel)
                .ProvideValue(new DummyServiceProvider(this, DataContextProperty));
        }

        internal ContentWindow(ContentWindowViewModel dataContext)
        {
            this.InitializeComponent();
            this.SetDataContext(dataContext);
        }

        internal ContentWindow(ContentWindowViewModel dataContext, DataTemplate dataTemplate)
        {
            this.InitializeComponent();
            this.SetDataContext(dataContext);
            this.ContentTemplate = dataTemplate;
        }

        internal ContentWindow(ContentWindowViewModel dataContext, DataTemplateSelector templateSelector)
        {
            this.InitializeComponent();
            this.SetDataContext(dataContext);
            this.ContentTemplateSelector = templateSelector;
        }

        internal ContentWindow(ContentWindowViewModel dataContext, ViewOption option)
        {
            InitializeComponent();
            SetDataContext(dataContext);

            if (option.Width.HasValue)
                Width = option.Width.Value;

            if (option.Height.HasValue)
                Height = option.Height.Value;

            if (option.ResizeMode.HasValue)
                ResizeMode = option.ResizeMode.Value;

            if (option.SizeToContent.HasValue)
                SizeToContent = option.SizeToContent.Value;

            if (option.WindowChrome != null)
                WindowChrome.SetWindowChrome(this, option.WindowChrome);

            if (option.Style.HasValue)
                WindowStyle = option.Style.Value;

            if (option.State.HasValue)
                WindowState = WindowStatus.ConvertWindowState(option.State.Value);

            if (option.StartupLocation.HasValue)
                WindowStartupLocation = option.StartupLocation.Value;

            if (option.ShowInTaskbar.HasValue)
                ShowInTaskbar = option.ShowInTaskbar.Value;
        }

        internal ContentWindow(ContentWindowViewModel dataContext, ViewOption option, DataTemplate dataTemplate)
            : this(dataContext, option)
        {
            ContentTemplate = dataTemplate;
        }

        internal ContentWindow(ContentWindowViewModel dataContext, ViewOption option, DataTemplateSelector templateSelector)
            : this(dataContext, option)
        {
            this.ContentTemplateSelector = templateSelector;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !(this.GetValue(FrameworkElement.DataContextProperty) as ContentWindowViewModel)?.CanClose() ?? false;

            base.OnClosing(e);
        }

        public class DummyServiceProvider : IProvideValueTarget, IServiceProvider
        {
            private readonly DependencyObject _targetObject;
            private readonly DependencyProperty _targetProperty;

            public DummyServiceProvider(DependencyObject targetObj, DependencyProperty targetProp)
            {
                this._targetObject = targetObj;
                this._targetProperty = targetProp;
            }

            object IProvideValueTarget.TargetObject => this._targetObject;

            object IProvideValueTarget.TargetProperty => this._targetProperty;

            public object GetService(Type serviceType)
            {
                return serviceType == typeof(IProvideValueTarget) ? this : null;
            }
        }
    }
}
