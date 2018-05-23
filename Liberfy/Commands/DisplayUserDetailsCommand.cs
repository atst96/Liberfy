using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Liberfy.View;
using Liberfy.ViewModel;

namespace Liberfy.Commands
{
    internal class DisplayUserDetailsCommand : Command<UserInfo>
    {
        private Account _account;

        public DisplayUserDetailsCommand(Account account)
        {
            this._account = account;
        }

        protected override bool CanExecute(UserInfo parameter) => true;

        protected override void Execute(UserInfo info)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is UserWindow
                    && window.DataContext is UserWindowViewModel vm
                    && vm.Account == this._account
                    && vm.User == info)
                {
                    window.Activate();
                    return;
                }
            }

            var view = new UserWindow()
            {
                DataContext = new UserWindowViewModel(info, this._account),
            };

            view.Show();
        }
    }
}
