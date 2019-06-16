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
using Liberfy.Model;
using Liberfy.ViewModels;

namespace Liberfy.Views
{
    /// <summary>
    /// MediaPreviewWindow.xaml の相互作用ロジック
    /// </summary>
    internal partial class MediaPreviewWindow : Window
    {
        private MediaPreviewWindowViewModel _viewModel;

        public MediaPreviewWindow()
        {
            this.InitializeComponent();

            this._viewModel = this.DataContext as MediaPreviewWindowViewModel;
        }

        internal MediaPreviewWindow(MediaAttachmentInfo mediaItem)
            : this()
        {
            this.SetItem(mediaItem);
        }

        private void SetItem(MediaAttachmentInfo mediaItem)
        {
            this._viewModel?.SetMediaItemInfo(mediaItem);
        }
    }
}
