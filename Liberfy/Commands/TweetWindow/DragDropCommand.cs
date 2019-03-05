using Liberfy.ViewModels;
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
        private TweetWindowViewModel _viewModel;

        public DragDropCommand(TweetWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        private readonly static string[] UrlDataPresets = { "IESiteModeToUrl", "text/x-moz-url", "UniformResourceLocator" };

        protected override bool CanExecute(IDataObject parameter)
        {
            var data = parameter;

            if (this._viewModel.IsBusy)
            {
                return false;
            }

            if (IsFileDropData(data))
            {
                this._viewModel.DropDescriptionMessage = "添付";
                this._viewModel.DragDropEffects = DragDropEffects.Copy;
                this._viewModel.DropDescriptionIcon = DropImageType.Copy;
            }
            else if (IsUrlData(data))
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
            if (DragDropCommand.TryGetStringDataType(parameter, out var type))
            {
                this._viewModel.TextBoxController.Insert((string)parameter.GetData(type));
                this._viewModel.TextBoxController.Focus();
            }
            else if (parameter.GetDataPresent(DataFormats.FileDrop))
            {
                var droppedFiles = (string[])parameter.GetData(DataFormats.FileDrop);
                this._viewModel.PostParameters.Attachments.AddRange(GetEnableMediaFiles(droppedFiles).Select(file => UploadMedia.FromFile(file)));
                this._viewModel.UpdateCanPost();
            }
        }

        private static bool IsFileDropData(IDataObject dataObject)
        {
            return dataObject.GetDataPresent(DataFormats.FileDrop)
                && dataObject.GetData(DataFormats.FileDrop) is string[] files
                && TweetWindowViewModel.HasEnableMediaFiles(files);
        }

        private static bool IsUrlData(IDataObject dataObject)
        {
            return UrlDataPresets.Any(key => dataObject.GetDataPresent(key))
                && dataObject.GetDataPresent(DataFormats.UnicodeText)
                && dataObject.GetDataPresent(DataFormats.Text);
        }

        private static bool TryGetStringDataType(IDataObject dataObject, out string type)
        {
            if (dataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                type = DataFormats.UnicodeText;
            }
            else if (dataObject.GetDataPresent(DataFormats.Text))
            {
                type = DataFormats.Text;
            }
            else if (UrlDataPresets.Any(f => dataObject.GetDataPresent(f)))
            {
                type = DataFormats.UnicodeText;
            }
            else
            {
                type = null;
            }

            return type != null;
        }

        private static IEnumerable<string> GetEnableMediaFiles(IEnumerable<string> files)
        {
            return files.Where(f => TweetWindowViewModel.IsUploadableExtension(Path.GetExtension(f)));
        }
    }
}
