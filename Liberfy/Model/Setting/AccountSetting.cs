using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Liberfy.Settings
{
    [DataContract]
    internal class AccountSetting : NotificationObject
    {
        public bool ContainsId(double id) => App.Accounts.Any(a => a.Id == id);

        public bool TryGetAccount(long id, out Account account)
        {
            account = App.Accounts.FirstOrDefault(a => a.Id == id);
            return account != null;
        }

        public Account GetAccount(long id) => App.Accounts.FirstOrDefault(a => a.Id == id);

        [DataMember(Name = "accounts")]
        private IEnumerable<AccountItem> _accounts;
        [IgnoreDataMember]
        public IEnumerable<AccountItem> Accounts
        {
            get => this._accounts ?? Enumerable.Empty<AccountItem>();
            set => _accounts = value;
        }

        
    }
}
