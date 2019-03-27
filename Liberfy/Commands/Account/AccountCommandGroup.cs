using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Commands.Account;

namespace Liberfy.Commands
{
    internal class AccountCommandGroup
    {
        private readonly IAccount _account;

        public Command<UserInfo> OpenUserWindowCommand { get; }

        public AccountCommandGroup(IAccount account)
        {
            this._account = account;

            this.OpenUserWindowCommand = new OpenUserWindowCommand(this._account);
        }
    }
}
