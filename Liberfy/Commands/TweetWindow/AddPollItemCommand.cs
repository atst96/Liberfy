using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.ViewModels;
using WpfMvvmToolkit;

namespace Liberfy.Commands.TweetWindow
{
    internal class AddPollItemCommand : Command
    {
        private readonly TweetWindowViewModel _viewModel;

        public AddPollItemCommand(TweetWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(object parameter)
        {
            if (this._viewModel.ServiceConfiguration == null)
            {
                return true;
            }

            return this._viewModel.PostParameters.Polls.Count < this._viewModel.ServiceConfiguration.MaxPollsCount;
        }

        protected override void Execute(object parameter)
        {
            this._viewModel.PostParameters.Polls.Add(new PollItem());

            this._viewModel.AddPollItemCommand.RaiseCanExecute();
            this._viewModel.RemovePollItemCommand.RaiseCanExecute();
        }
    }
}
