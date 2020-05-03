
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Liberfy.ViewModels;
using Livet.Messaging;
using WpfMvvmToolkit;

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

            var message = new ConfirmationMessage(
                $"このアカウントを一覧から削除しますか？\n{user.Info.Name}@{user.Info.UserName}", App.Name,
                MessageBoxImage.Question, "MsgKey_ConfirmMessage");

            this._viewModel.Messenger.Raise(message);

            if (message.Response ?? false)
            {
                AccountManager.Remove(parameter);
                parameter.Unload();
            }

            this._viewModel.RaiseCanExecuteAccountCommands();
        }
    }
}
