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

namespace Liberfy.Views
{
    /// <summary>
    /// TweetWindow.xaml の相互作用ロジック
    /// </summary>
    internal partial class TweetWindow : Window
    {
        public TweetWindow()
        {
            this.InitializeComponent();
        }

        internal TweetWindow(IAccount account) : this()
        {
            if (account != null)
            {
                var viewModel = DataContext as ViewModels.TweetWindowViewModel;

                viewModel?.SetPostAccount(account);
            }
        }
    }
}
