using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvmToolkit;

namespace Liberfy.Commands.Status
{
    internal class RetweetCommand : Command
    {
        private readonly IAccount _account;
        private readonly StatusItem _item;

        public RetweetCommand(StatusItem item)
        {
            this._account = item.Account;
            this._item = item;
        }

        protected override bool CanExecute(object parameter)
        {
            return this._account.Validator.CanRetweet(this._item);
        }

        protected override void Execute(object parameter)
        {
            var apiGateway = this._account.ApiGateway;
            var item = this._item;

            if (this._item.Reaction.IsRetweeted)
            {
                apiGateway.Unretweet(item);
            }
            else
            {
                apiGateway.Retweet(item);
            }
        }
    }
}
