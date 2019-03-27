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
using Liberfy.ViewModels;

namespace Liberfy.Views
{
    /// <summary>
    /// TweetWindow.xaml の相互作用ロジック
    /// </summary>
    internal partial class TweetWindow : Window
    {
        private TweetWindowViewModel _viewModel;

        public TweetWindow()
        {
            this.InitializeComponent();

            this._viewModel = this.DataContext as TweetWindowViewModel;
        }

        public TweetWindow(IAccount account) : this()
        {
            this._viewModel?.SetPostAccount(account);
        }

        public TweetWindow(StatusItem statusItem) : this(statusItem.Account)
        {
            this._viewModel?.SetReplyToStatus(statusItem);
        }
    }
}
