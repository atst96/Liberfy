using Liberfy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvmToolkit;
using static Liberfy.Defaults;

namespace Liberfy.Commands
{
    internal class AddImageCommand : Command<string>
    {
        private TweetWindowViewModel _viewModel;

        public AddImageCommand(TweetWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        private static readonly IDictionary<string, string[]> UploadableExtensionFilter = CreateExtensionFilter();

        protected override bool CanExecute(string parameter)
        {
            return !this._viewModel.IsUploading;
        }

        protected override void Execute(string parameter)
        {
            var files = this._viewModel.DialogService.SelectOpenFiles("アップロードするメディアを選択", UploadableExtensionFilter);

            if (files?.Any() == true && TweetWindowViewModel.HasEnableMediaFiles(files))
            {
                this._viewModel.PostParameters.Attachments.AddRange(files.Select(path => UploadMedia.FromFile(path)));
                this._viewModel.UpdateCanPost();
            }
        }

        private static IDictionary<string, string[]> CreateExtensionFilter()
        {
            // OpenFileDialogで用いる拡張子フィルタの生成
            // e.g. 表示名|*.ext1|表示名(拡張子複数指定)|*.ext2;*.ext2|...

            return new Dictionary<string, string[]>
            {
                ["アップロード可能なメディア"] = UploadableMediaExtensions,
                ["画像ファイル"] = ImageExtensions,
                ["動画ファイル"] = VideoExtensions,
                ["すべてのファイル"] = new[] { ".*" },
            };
        }
    }
}
