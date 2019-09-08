using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Liberfy.Model;
using Liberfy.Settings;
using SocialApis.Twitter;

namespace Liberfy.Services.Twitter
{
    internal class TwitterDataStore : DataStoreBase<User, Status>
    {
        public override IUserInfo<User> GetAccount(AccountSettingBase item)
        {
            var twitterItem = (TwitterAccountItem)item;
            return this.Accounts.GetOrAdd(twitterItem.Id, id => new TwitterUserInfo(twitterItem));
        }

        public override IUserInfo<User> RegisterAccount(User account)
        {
            long id = account.Id ?? throw new ArgumentException(nameof(account));

            return this.Accounts.AddOrUpdate(id,
                (_) => new TwitterUserInfo(account),
                (_, info) => info.Update(account));
        }

        public override IStatusInfo<Status> RegisterStatus(Status status)
        {
            long id = status?.Id ?? throw new ArgumentException(nameof(status));

            return this.Statuses.AddOrUpdate(id,
                (_) => new TwitterStatusInfo(status, this),
                (_, info) => info.Update(status));
        }
    }
}
