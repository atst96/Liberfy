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
        private static readonly NotifiableCollection<IAccount> _accounts = new NotifiableCollection<IAccount>();
        public static IEnumerable<IAccount> Accounts { get; } = _accounts;

        public static readonly IDictionary<ServiceType, IDictionary<long, IAccount>> _serviceAccountMap = new Dictionary<ServiceType, IDictionary<long, IAccount>>();

        public static void Load(IEnumerable<AccountItem> accounts)
        {
            foreach (var accountSetting in accounts.Distinct())
            {
                Add(AccountBase.FromSetting(accountSetting));
            }
        }

        public static IAccount Get(ServiceType service, long userId)
        {
            if (_serviceAccountMap.TryGetValue(service, out var dic))
            {
                if (dic.TryGetValue(userId, out var account))
                    return account;
            }

            return null;
        }

        public static void Add(IAccount account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            GetAccountList(account.Service).Add(account.Id, account);
            _accounts.Add(account);
        }

        public static bool Contains(ServiceType service, long id)
        {
            return GetAccountList(service).ContainsKey(id);
        }

        public static bool Contains(IAccount account)
        {
            return account != null && Contains(account.Service, account.Id);
        }

        public static void Remove(IAccount account)
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

        public static int IndexOf(IAccount account)
        {
            return _accounts.IndexOf(account);
        }

        public static int Count => _accounts.Count;

        private static IDictionary<long, IAccount> GetAccountList(ServiceType service)
        {
            IDictionary<long, IAccount> dic;

            if (_serviceAccountMap.TryGetValue(service, out dic))
            {
                return dic;
            }
            else
            {
                dic = new Dictionary<long, IAccount>();
                _serviceAccountMap.Add(service, dic);
                return dic;
            }
        }
    }
}
