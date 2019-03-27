using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.ViewModels;

namespace Liberfy.Commands.TweetWindow
{
    internal class RemovePollItemCommand : Command
    {
        private readonly TweetWindowViewModel _viewModel;

        public RemovePollItemCommand(TweetWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(object parameter)
        {
            return this._viewModel.PostParameters.Polls.Count > 2;
        }

        protected override void Execute(object parameter)
        {
            var items = this._viewModel.PostParameters.Polls;

            items.RemoveAt(items.Count - 1);

            this._viewModel.AddPollItemCommand.RaiseCanExecute();
            this._viewModel.RemovePollItemCommand.RaiseCanExecute();
        }
    }
}
