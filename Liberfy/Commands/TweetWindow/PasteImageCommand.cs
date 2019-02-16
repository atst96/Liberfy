using Liberfy.ViewModel;
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
        private TweetWindow _viewModel;

        public PasteImageCommand(TweetWindow viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(object parameter)
        {
            return Clipboard.ContainsImage() || (Clipboard.ContainsFileDropList() && TweetWindow.HasEnableMediaFiles(Clipboard.GetFileDropList()));
        }

        protected override void Execute(object parameter)
        {
            if (Clipboard.ContainsImage())
            {
                this._viewModel.PostParameters.Attachments.Add(UploadMedia.FromBitmapSource(Clipboard.GetImage()));
            }
            else if (Clipboard.ContainsFileDropList())
            {
                this._viewModel.PostParameters.Attachments.AddRange(
                    GetEnableMediaFiles(Clipboard.GetFileDropList())
                    .Select(file => UploadMedia.FromFile(file)));
            }
        }

        private static IEnumerable<string> GetEnableMediaFiles(StringCollection strCollection)
        {
            foreach (var str in strCollection)
            {
                if (TweetWindow.IsUploadableExtension(Path.GetExtension(str)))
                    yield return str;
            }

            yield break;
        }
    }
}
