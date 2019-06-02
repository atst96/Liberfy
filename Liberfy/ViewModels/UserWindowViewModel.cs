using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.ViewModels
{
    internal class UserWindowViewModel : ViewModelBase
    {
        public UserWindowViewModel(IUserInfo user, IAccount account)
        {
            this.User = user;
            this.Account = account;
        }

        public IAccount Account { get; }

        public IUserInfo User { get; }
    }
}
