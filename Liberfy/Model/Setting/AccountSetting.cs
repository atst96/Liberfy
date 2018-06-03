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
        public bool ContainsId(double id) => App.Accounts.Any(a => a.Id == id);

        public bool TryGetAccount(SocialService service, long id, out AccountBase account)
        {
            account = App.Accounts.FirstOrDefault(a => a.Service == service && a.Id == id);
            return account != null;
        }

        public AccountBase GetAccount(SocialService service, long id)
        {
            return App.Accounts.FirstOrDefault(a => a.Service == service && a.Id == id);
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
