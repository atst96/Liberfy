using Liberfy.Commands;
using Liberfy.Twitter.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Liberfy.Twitter
{
    internal class AccountCommandGroup : IAccountCommandGroup
    {
        private TwitterAccount _account;

        public AccountCommandGroup(TwitterAccount account)
        {
            this._account = account;
        }

        private ICommand _repost;
        public ICommand Repost => this._repost ?? (this._repost = new RepostCommand());

        private ICommand _like;
        public ICommand Like => this._like ?? (this._like = new LikeCommand());

        private ICommand _displayUserDetails;
        public ICommand DisplayUserDetails => this._displayUserDetails ?? (this._displayUserDetails = new DisplayUserDetailsCommand(this._account));
    }
}
