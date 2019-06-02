using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Liberfy.ViewModels;
using Liberfy.Views;
using WpfMvvmToolkit;

namespace Liberfy.Commands.Account
{
    internal class OpenUserWindowCommand : Command<IUserInfo>
    {
        private readonly IAccount _account;

        public OpenUserWindowCommand(IAccount account)
        {
            this._account = account;
        }

        protected override bool CanExecute(IUserInfo parameter)
        {
            return true;
        }

        protected override void Execute(IUserInfo parameter)
        {
            var (view, viewModel) = App.Instance.FindViewModelWithWindow<UserWindowViewModel>()
                .FirstOrDefault(item => item.viewModel.Account.Equals(this._account) && item.viewModel.User.Equals(parameter));

            if (view != null)
            {
                view.Activate();
                return;
            }

            view = new UserWindow()
            {
                DataContext = new UserWindowViewModel(parameter, this._account),
            };

            view.Show();
        }
    }
}
