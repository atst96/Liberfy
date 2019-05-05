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
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    internal partial class SettingWindow : CustomWindow
    {
        public SettingWindow()
        {
            this.InitializeComponent();
        }

        public SettingWindow(int? page) : base()
        {
            if (page.HasValue)
            {
                this.MoveTabPage(page.Value);
            }
        }

        public void MoveTabPage(int index)
        {
            this.tabControl.SelectedIndex = index;
        }
    }
}
