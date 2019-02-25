using Liberfy.ViewModel;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Commands
{
    internal class PostCommand : Command<IAccount>
    {
        private TweetWindow _viewModel;

        public PostCommand(TweetWindow viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(IAccount parameter)
        {
            return parameter != null && this._viewModel.CanPostTweet();
        }

        protected override async void Execute(IAccount account)
        {
            var parameters = this._viewModel.PostParameters;

            this._viewModel.UploadStatusText = "ツイートしています...";

            this._viewModel.BeginUpload();

            foreach (var attachment in parameters.Attachments)
            {
                attachment.BeginUpload();
            }

            try
            {
                await account.ApiGateway.PostStatus(parameters);
                this._viewModel.ClearStatus();
            }
            catch (Exception ex)
            {
                this._viewModel.DialogService.MessageBox(ex.GetMessage(), "アップロードに失敗しました");
            }

            foreach (var attachment in parameters.Attachments)
            {
                attachment.EndUpload();
            }

            this._viewModel.EndUpload();
        }
    }
}
