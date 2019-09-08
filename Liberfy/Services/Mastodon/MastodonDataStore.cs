using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Liberfy.Model;
using Liberfy.Settings;
using SocialApis.Mastodon;

namespace Liberfy.Services.Mastodon
{
    internal class MastodonDataStore : DataStoreBase<Account, Status>
    {
        private readonly Uri _host;

        public MastodonDataStore(Uri host)
        {
            this._host = host;
        }

        public override IUserInfo<Account> GetAccount(AccountSettingBase item)
        {
            var mastodonItem = (MastodonAccountItem)item;
            return this.Accounts.GetOrAdd(mastodonItem.Id, id => new MastodonUserInfo(this._host, mastodonItem));
        }

        public override IUserInfo<Account> RegisterAccount(Account account)
        {
            long id = account?.Id ?? throw new ArgumentException(nameof(account));

            return this.Accounts.AddOrUpdate(id,
                (_) => new MastodonUserInfo(this._host, account),
                (_, info) => info.Update(account));
        }

        public override IStatusInfo<Status> RegisterStatus(Status status)
        {
            long id = status?.Id ?? throw new ArgumentException(nameof(status));

            return this.Statuses.AddOrUpdate(id,
                (_) => new MastodonStatusInfo(status, this),
                (_, info) => info.Update(status));
        }
    }
}
