using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Liberfy.Commands
{
    internal class AccountCommands
    {
        private Account _account;

        public AccountCommands(Account account)
        {
            this._account = account;
        }

        private ICommand _displayUserDetails;
        public ICommand DisplayUserDetails => this._displayUserDetails ?? (this._displayUserDetails = new DisplayUserDetailsCommand(this._account));
    }
}
