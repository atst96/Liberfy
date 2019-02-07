
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.ViewModel;

namespace Liberfy.Commands.SettingWindowCommands
{
    internal class AccountDeleteCommand : Command<IAccount>
    {
        private readonly SettingWindowViewModel _viewModel;

        public AccountDeleteCommand(SettingWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(IAccount parameter)
        {
            return AccountManager.Contains(parameter);
        }

        protected override void Execute(IAccount parameter)
        {
            var user = parameter;

            var message = $"このアカウントを一覧から削除しますか？\n { user.Info.Name }@{ user.Info.ScreenName }";

            if (this._viewModel.DialogService.ShowQuestion(message))
            {
                AccountManager.Remove(parameter);
                parameter.Unload();
            }

            this._viewModel.RaiseCanExecuteAccountCommands();
        }
    }
}
