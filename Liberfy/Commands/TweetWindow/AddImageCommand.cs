using Liberfy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private static readonly string UploadableExtensionFilter = CreateExtensionFilter();

        protected override bool CanExecute(string parameter)
        {
            return !this._viewModel.IsUploading;
        }

        protected override void Execute(string parameter)
        {
            var files = this._viewModel.DialogService.SelectOpenFiles("アップロードするメディアを選択", UploadableExtensionFilter);

            if (files?.Length > 0 && TweetWindowViewModel.HasEnableMediaFiles(files))
            {
                this._viewModel.PostParameters.Attachments.AddRange(files.Select(path => UploadMedia.FromFile(path)));
                this._viewModel.UpdateCanPost();
            }
        }

        private static string CreateExtensionFilter()
        {
            // OpenFileDialogで用いる拡張子フィルタの生成
            // e.g. 表示名|*.ext1|表示名(拡張子複数指定)|*.ext2;*.ext2|...

            var medExts = $"アップロード可能なメディア|*{string.Join(";*", UploadableMediaExtensions)}";
            var imgExts = $"画像ファイル|*{string.Join(";*", ImageExtensions)}";
            var vidExts = $"動画ファイル|*{string.Join(";*", VideoExtensions)}";
            var allExts = "すべてのファイル|*.*";

            // 上記の文字列を‘|’(縦線)を区切り文字として結合
            return string.Join("|", medExts, imgExts, vidExts, allExts);
        }
    }
}
