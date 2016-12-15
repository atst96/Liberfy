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

		internal ContentWindow(ContentWindowViewModel dataContext)
		{
			InitializeComponent();
			DataContext = dataContext;
		}

		internal ContentWindow(ContentWindowViewModel dataContext, DataTemplate dataTemplate)
		{
			InitializeComponent();
			DataContext = dataContext;
			ContentTemplate = dataTemplate;
		}

		internal ContentWindow(ContentWindowViewModel dataContext, DataTemplateSelector templateSelector)
		{
			InitializeComponent();
			DataContext = dataContext;
			ContentTemplateSelector = templateSelector;
		}

		internal ContentWindow(ContentWindowViewModel dataContext, ViewOption option)
		{
			InitializeComponent();
			DataContext = dataContext;

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
				WindowState = option.State.Value;

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
	}
}
