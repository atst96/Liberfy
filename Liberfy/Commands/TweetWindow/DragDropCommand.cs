using Liberfy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Liberfy.Commands
{
    internal class DragDropCommand : Command<IDataObject>
    {
        private TweetWindow _viewModel;

        public DragDropCommand(TweetWindow viewModel)
        {
            this._viewModel = viewModel;
        }

        private readonly static string[] UrlDataPresets = { "IESiteModeToUrl", "text/x-moz-url", "UniformResourceLocator" };

        protected override bool CanExecute(IDataObject parameter)
        {
            var data = parameter;

            if (this._viewModel.IsBusy) return false;

            if (data.GetDataPresent(DataFormats.FileDrop)
                && data.GetData(DataFormats.FileDrop) is string[] dropFiles
                && TweetWindow.HasEnableMediaFiles(dropFiles))
            {
                this._viewModel.DropDescriptionMessage = "添付";
                this._viewModel.DragDropEffects = DragDropEffects.Copy;
                this._viewModel.DropDescriptionIcon = DropImageType.Copy;
            }
            else if (UrlDataPresets.Any(data.GetDataPresent)
                || data.GetDataPresent(DataFormats.UnicodeText)
                || data.GetDataPresent(DataFormats.Text))
            {
                this._viewModel.DropDescriptionMessage = "挿入";
                this._viewModel.DragDropEffects = DragDropEffects.Copy;
                this._viewModel.DropDescriptionIcon = DropImageType.Label;
            }
            else
            {
                this._viewModel.DropDescriptionMessage = "無効な形式";
                this._viewModel.DragDropEffects = DragDropEffects.None;
                this._viewModel.DropDescriptionIcon = DropImageType.None;
                return false;
            }

            return true;
        }

        protected override void Execute(IDataObject parameter)
        {
            var data = parameter;

            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                var droppedFiles = (string[])data.GetData(DataFormats.FileDrop);
                this._viewModel.PostParameters.Attachments.AddRange(GetEnableMediaFiles(droppedFiles).Select(file => UploadMedia.FromFile(file)));
                this._viewModel.UpdateCanPost();
            }
            else if (UrlDataPresets.Any(data.GetDataPresent))
            {
                this._viewModel.TextBoxController.Insert((string)data.GetData(DataFormats.UnicodeText));
                this._viewModel.TextBoxController.Focus();
            }
            else if (data.GetDataPresent(DataFormats.UnicodeText))
            {
                this._viewModel.TextBoxController.Insert((string)data.GetData(DataFormats.UnicodeText));
                this._viewModel.TextBoxController.Focus();
            }
            else if (data.GetDataPresent(DataFormats.Text))
            {
                this._viewModel.TextBoxController.Insert((string)data.GetData(DataFormats.Text));
                this._viewModel.TextBoxController.Focus();
            }
        }

        private static IEnumerable<string> GetEnableMediaFiles(IEnumerable<string> files)
        {
            return files.Where(f => TweetWindow.IsUploadableExtension(Path.GetExtension(f)));
        }
    }
}
