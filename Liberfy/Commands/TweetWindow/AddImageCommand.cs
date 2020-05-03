using Liberfy.ViewModels;
using Livet.Messaging.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvmToolkit;
using static Liberfy.Defaults;

namespace Liberfy.Commands
{
    internal class AddImageCommand : Command<OpeningFileSelectionMessage>
    {
        private TweetWindowViewModel _viewModel;

        public AddImageCommand(TweetWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(OpeningFileSelectionMessage parameter)
        {
            return parameter.Response?.Length > 0;
        }

        protected override void Execute(OpeningFileSelectionMessage parameter)
        {
            var files = parameter.Response;

            if (files?.Any() == true && TweetWindowViewModel.HasEnableMediaFiles(files))
            {
                this._viewModel.PostParameters.Attachments.AddRange(files.Select(path => UploadMedia.FromFile(path)));
                this._viewModel.UpdateCanPost();
            }
        }
    }
}
