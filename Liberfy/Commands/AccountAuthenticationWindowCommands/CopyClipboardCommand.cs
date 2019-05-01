using Liberfy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfMvvmToolkit;

namespace Liberfy.Commands.AccountAuthenticationWindowCommands
{
    internal class CopyClipboardCommand : Command
    {
        private readonly AccountAuthenticationWindowViewModel _viewModel;

        public CopyClipboardCommand(AccountAuthenticationWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void Execute(object parameter)
        {
            var authenticator = this._viewModel.AccountAuthenticator;

            if (authenticator != null)
            {
                Clipboard.SetText(authenticator.AuthorizeUrl);
            }
        }
    }
}
