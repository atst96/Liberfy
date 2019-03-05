using Liberfy.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Liberfy.Commands
{
    internal class PasteImageCommand : Command
    {
        private TweetWindowViewModel _viewModel;

        public PasteImageCommand(TweetWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(object parameter)
        {
            return Clipboard.ContainsImage() || (Clipboard.ContainsFileDropList() && TweetWindowViewModel.HasEnableMediaFiles(Clipboard.GetFileDropList()));
        }

        protected override void Execute(object parameter)
        {
            if (Clipboard.ContainsImage())
            {
                var attachment = UploadMedia.FromBitmapSource(Clipboard.GetImage(), UploadMedia.DisplayExtensions.Clipboard);

                this._viewModel.PostParameters.Attachments.Add(attachment);
            }
            else if (Clipboard.ContainsFileDropList())
            {
                var files = Clipboard.GetFileDropList();


                this._viewModel.PostParameters.Attachments.AddRange(
                    GetEnableMediaFiles(files)
                    .Select(file => UploadMedia.FromFile(file)));
            }
        }

        private static IEnumerable<string> GetEnableMediaFiles(StringCollection collection)
        {
            foreach (var str in collection)
            {
                if (TweetWindowViewModel.IsUploadableExtension(Path.GetExtension(str)))
                    yield return str;
            }
        }
    }
}
