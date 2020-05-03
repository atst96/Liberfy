using Liberfy.ViewModels;
using Livet.Messaging.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvmToolkit;

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
            this._viewModel.Messenger.Raise(new WindowActionMessage(WindowAction.Close, "MsgKey_WindowAction"));
        }
    }
}
