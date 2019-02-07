using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.ViewModel
{
    internal class UserWindowViewModel : ViewModelBase
    {
        public UserWindowViewModel(UserInfo user, IAccount account)
        {
            this.User = user;
            this.Account = account;
        }

        public IAccount Account { get; }

        public UserInfo User { get; }
    }
}
