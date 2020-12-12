using System.Windows;
using Liberfy.Managers;
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
            return AccountManager.Accounts.Contains(parameter);
        }

        protected override void Execute(IAccount parameter)
        {
            var user = parameter;

            var message = new ConfirmationMessage(
                //$"このアカウントを一覧から削除しますか？\n{user.Info.Name}@{user.Info.UserName}", App.Name,
                $"このアカウントを一覧から削除しますか？", App.Name,
                MessageBoxImage.Question, "MsgKey_ConfirmMessage");

            this._viewModel.Messenger.Raise(message);

            if (message.Response ?? false)
            {
                AccountManager.Accounts.Remove(parameter);
                parameter.StopActivity();
            }

            this._viewModel.RaiseCanExecuteAccountCommands();
        }
    }
}
