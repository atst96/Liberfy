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
            InitializeComponent();
        }

        private void setDataContext(ContentWindowViewModel viewModel)
        {
            DataContext = new ViewModelConnector(viewModel).
                ProvideValue(new DummyServiceProvider(this, DataContextProperty));
        }

        internal ContentWindow(ContentWindowViewModel dataContext)
        {
            InitializeComponent();
            setDataContext(dataContext);
        }

        internal ContentWindow(ContentWindowViewModel dataContext, DataTemplate dataTemplate)
        {
            InitializeComponent();
            setDataContext(dataContext);
            ContentTemplate = dataTemplate;
        }

        internal ContentWindow(ContentWindowViewModel dataContext, DataTemplateSelector templateSelector)
        {
            InitializeComponent();
            setDataContext(dataContext);
            ContentTemplateSelector = templateSelector;
        }

        internal ContentWindow(ContentWindowViewModel dataContext, ViewOption option)
        {
            InitializeComponent();
            setDataContext(dataContext);

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
            ContentTemplateSelector = templateSelector;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !(GetValue(DataContextProperty) as ContentWindowViewModel)?.CanClose() ?? false;

            base.OnClosing(e);
        }

        public class DummyServiceProvider : IProvideValueTarget, IServiceProvider
        {
            private readonly DependencyObject _targetObject;
            private readonly DependencyProperty _targetProperty;

            public DummyServiceProvider(DependencyObject targetObj, DependencyProperty targetProp)
            {
                _targetObject = targetObj;
                _targetProperty = targetProp;
            }

            object IProvideValueTarget.TargetObject => _targetObject;

            object IProvideValueTarget.TargetProperty => _targetProperty;

            public object GetService(Type serviceType)
            {
                return serviceType == typeof(IProvideValueTarget) ? this : null;
            }
        }
    }
}
