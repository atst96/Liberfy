using Liberfy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvmToolkit;

namespace Liberfy.Commands.SettingWindowCommands
{
    internal class AccountMoveUpCommand : Command<IAccount>
    {
        private readonly SettingWindowViewModel _viewModel;

        public AccountMoveUpCommand(SettingWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(IAccount parameter)
        {
            return AccountManager.IndexOf(parameter) > 1;
        }

        protected override void Execute(IAccount parameter)
        {
            int index = AccountManager.IndexOf(parameter);
            AccountManager.Move(index, index - 1);

            this._viewModel.RaiseCanExecuteAccountCommands();
        }
    }
}
