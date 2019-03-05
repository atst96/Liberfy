using Liberfy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Commands.AccountAuthenticationWindowCommands
{
    internal class CancelCommand : Command
    {
        private AccountAuthenticationWindowViewModel _viewModel;

        public CancelCommand(AccountAuthenticationWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(object parameter)
        {
            return !this._viewModel.IsRunning;
        }

        protected override void Execute(object parameter)
        {
            this._viewModel.DialogService.Close(false);
        }
    }
}
