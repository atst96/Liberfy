using System;
using Liberfy.Data.Mastodon;
using Liberfy.Settings;
using SocialApis.Mastodon;

namespace Liberfy.Factories
{
    internal class MastodonDataFactory : DataStoreBase<Account, Status>
    {
        private readonly Uri _host;

        public MastodonDataFactory(Uri host)
        {
            this._host = host;
        }

        public override IUserInfo<Account> GetAccount(AccountSettingBase item)
        {
            var mastodonItem = (MastodonAccountItem)item;
            return this.Accounts.GetOrAdd(mastodonItem.Id, id => new AccountDetail(this._host, mastodonItem));
        }

        public override IUserInfo<Account> RegisterAccount(Account account)
        {
            long id = account?.Id ?? throw new ArgumentException(nameof(account));

            return this.Accounts.AddOrUpdate(id,
                (_) => new AccountDetail(this._host, account),
                (_, info) => info.Update(account));
        }

        public override IStatusInfo<Status> RegisterStatus(Status status)
        {
            long id = status?.Id ?? throw new ArgumentException(nameof(status));

            return this.Statuses.AddOrUpdate(id,
                (_) => new TootDetail(status, this),
                (_, info) => info.Update(status));
        }
    }
}
