using Liberfy.ViewModel;
using SocialApis;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Liberfy.Settings
{
    [DataContract]
    internal class AccountSetting : NotificationObject
    {
        public bool ContainsId(long id)
        {
            foreach (var account in App.Accounts)
            {
                if (account.Id == id)
                    return true;
            }

            return false;
        }

        public bool TryGetAccount(SocialService service, long id, out AccountBase account)
        {
            account = this.GetAccount(service, id);

            return account != null;
        }

        public AccountBase GetAccount(SocialService service, long id)
        {
            foreach (var account in App.Accounts)
            {
                if (account.Service == service && account.Id == id)
                {
                    return account;
                }
            }

            return null;
        }

        [DataMember(Name = "accounts")]
        private IEnumerable<AccountItem> _accounts;
        [IgnoreDataMember]
        public IEnumerable<AccountItem> Accounts
        {
            get => this._accounts ?? Enumerable.Empty<AccountItem>();
            set => this._accounts = value;
        }
    }
}
