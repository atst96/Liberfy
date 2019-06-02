using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvmToolkit;

namespace Liberfy.Commands.Status
{
    internal class FavoriteCommand : Command
    {
        private readonly IAccount _account;
        private readonly StatusItem _item;

        public FavoriteCommand(StatusItem item)
        {
            this._account = item.Account;
            this._item = item;
        }

        protected override bool CanExecute(object parameter)
        {
            return this._account.Validator.CanFavorite(this._item);
        }

        protected override void Execute(object parameter)
        {
            var apiGateway = this._account.ApiGateway;

            if (this._item.Reaction.IsRetweeted)
            {
                apiGateway.Unfavorite(this._item);
            }
            else
            {
                apiGateway.Favorite(this._item);
            }
        }
    }
}
