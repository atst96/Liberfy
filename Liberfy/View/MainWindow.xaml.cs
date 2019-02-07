using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Liberfy
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //switch (RenderCapability.Tier >> 16)
            //{
            //    case 0:
            //        Title += " [SWレンダリング]"; break;

            //    case 1:
            //        Title += " [HWレンダリング(制限)]"; break;

            //    case 2:
            //        Title += " [HWレンダリング]"; break;
            //}
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var status = App.Setting.Window.Main;


            if (status.Top.HasValue)
                this.Top = status.Top.Value;

            if (status.Left.HasValue)
                this.Left = status.Left.Value;

            if (status.Width.HasValue)
                this.Width = status.Width.Value;

            if (status.Height.HasValue)
                this.Height = status.Height.Value;

            if (App.Setting.MinimizeStartup)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }
            else if (!status.State.HasValue && status.State != Liberfy.WindowState.Minimized)
            {
                // WindowStateの設定はウィンドウ表示直後(Loadedイベント呼び出し後)に行う
                // (マルチディスプレイ環境においてWindowState.Maximizedを元のディスプレイで復元させるため)

                this.Loaded += this.MainWindowLoadedSetState;
            }
        }

        private void MainWindowLoadedSetState(object sender, RoutedEventArgs e)
        {
            var status = App.Setting.Window.Main;

            this.Loaded -= this.MainWindowLoadedSetState;

            if (status.State.HasValue)
            {
                this.WindowState = WindowStatus.ConvertWindowState(status.State.Value);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var status = App.Setting.Window.Main;

            status.Top = this.Top;
            status.Left = this.Left;
            status.Width = this.Width;
            status.Height = this.Height;

            status.State = WindowStatus.ConvertWindowState(this.WindowState);

            if (App.Setting.MinimizeAtCloseButtonClick)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
                e.Cancel = true;
            }

            base.OnClosing(e);
        }
    }
}
