using System;
using Liberfy.Data.Twitter;
using Liberfy.Settings;
using SocialApis.Twitter;

namespace Liberfy.Factories
{
    internal class TwitterDataFactory : DataStoreBase<User, Status>
    {
        public override IUserInfo<User> GetAccount(AccountSettingBase item)
        {
            var twitterItem = (TwitterAccountItem)item;
            return this.Accounts.GetOrAdd(twitterItem.Id, id => new UserDetail(twitterItem));
        }

        public override IUserInfo<User> RegisterAccount(User account)
        {
            long id = account.Id ?? throw new ArgumentException(nameof(account));

            return this.Accounts.AddOrUpdate(id,
                (_) => new UserDetail(account),
                (_, info) => info.Update(account));
        }

        public override IStatusInfo<Status> RegisterStatus(Status status)
        {
            long id = status?.Id ?? throw new ArgumentException(nameof(status));

            return this.Statuses.AddOrUpdate(id,
                (_) => new TweetDetail(status, this),
                (_, info) => info.Update(status));
        }
    }
}
