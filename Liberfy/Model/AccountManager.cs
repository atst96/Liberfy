using Liberfy.Settings;
using SocialApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal static class AccountManager
    {
        private static readonly FluidCollection<AccountBase> _accounts = new FluidCollection<AccountBase>();
        public static IEnumerable<AccountBase> Accounts { get; } = _accounts;

        public static readonly IDictionary<SocialService, IDictionary<long, AccountBase>> _serviceAccountMap = new Dictionary<SocialService, IDictionary<long, AccountBase>>();

        public static void Load(IEnumerable<AccountItem> accounts)
        {
            foreach (var accountSetting in accounts.Distinct())
            {
                Add(AccountBase.FromSetting(accountSetting));
            }
        }

        public static AccountBase Get(SocialService service, long userId)
        {
            if (_serviceAccountMap.TryGetValue(service, out var dic))
            {
                if (dic.TryGetValue(userId, out var account))
                    return account;
            }

            return null;
        }

        public static void Add(AccountBase account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            GetAccountList(account.Service).Add(account.Id, account);
            _accounts.Add(account);
        }

        public static bool Contains(SocialService service, long id)
        {
            return GetAccountList(service).ContainsKey(id);
        }

        public static bool Contains(AccountBase account)
        {
            return account != null && Contains(account.Service, account.Id);
        }

        public static void Remove(AccountBase account)
        {
            if (_serviceAccountMap.TryGetValue(account.Service, out var dic))
            {
                if (dic.ContainsKey(account.Id))
                {
                    dic.Remove(account.Id);
                    _accounts.Remove(account);
                }
            }
        }

        public static void Move(int oldIndex, int newIndex)
        {
            _accounts.Move(oldIndex, newIndex);
        }

        public static int IndexOf(AccountBase account)
        {
            return _accounts.IndexOf(account);
        }

        public static int Count => _accounts.Count;

        private static IDictionary<long, AccountBase> GetAccountList(SocialService service)
        {
            IDictionary<long, AccountBase> dic;

            if (_serviceAccountMap.TryGetValue(service, out dic))
            {
                return dic;
            }
            else
            {
                dic = new Dictionary<long, AccountBase>();
                _serviceAccountMap.Add(service, dic);
                return dic;
            }
        }
    }
}
