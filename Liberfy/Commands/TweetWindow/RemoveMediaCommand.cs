using Liberfy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Commands
{
    internal class RemoveMediaCommand : Command<UploadMedia>
    {
        private TweetWindow _viewModel;

        public RemoveMediaCommand(TweetWindow viewModel) : base(true)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(UploadMedia parameter)
        {
            bool value = this._viewModel.PostParameters.Attachments.Contains(parameter);

            return this._viewModel.PostParameters.Attachments.Contains(parameter);
        }

        protected override void Execute(UploadMedia parameter)
        {
            using (parameter)
            {
                this._viewModel.PostParameters.Attachments.Remove(parameter);
                this._viewModel.UpdateCanPost();
            }
        }
    }
}
