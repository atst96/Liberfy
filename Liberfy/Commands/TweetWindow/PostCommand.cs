using Liberfy.ViewModels;
using Livet.Messaging;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfMvvmToolkit;

namespace Liberfy.Commands
{
    internal class PostCommand : Command<IAccount>
    {
        private TweetWindowViewModel _viewModel;

        public PostCommand(TweetWindowViewModel viewModel)
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
                this._viewModel.Messenger.Raise(new InformationMessage($"アップロードに失敗しました:\n{ex.GetMessage()}", App.Name, MessageBoxImage.Error, "MsgKey_InformationMessage"));
            }

            foreach (var attachment in parameters.Attachments)
            {
                attachment.EndUpload();
            }

            this._viewModel.EndUpload();
        }
    }
}
