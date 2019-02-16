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
            this._viewModel.UploadStatusText = "ツイートしています...";

            this._viewModel.BeginUpload();

            try
            {
                await account.ApiGateway.PostStatus(this._viewModel.PostParameters);
                this._viewModel.ClearStatus();
            }
            catch (AggregateException aex)
            {
                var exceptionMessages = aex.InnerExceptions.Select(ex => ex.Message);
                var message = string.Join("\n", exceptionMessages);

                this._viewModel.DialogService.MessageBox(message, "アップロードに失敗しました");
            }
            catch (Exception ex)
            {
                this._viewModel.DialogService.MessageBox(ex.Message, "アップロードに失敗しました");
            }

            this._viewModel.EndUpload();
        }
    }
}
