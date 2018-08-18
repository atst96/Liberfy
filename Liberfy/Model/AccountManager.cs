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
        public static IEnumerable<AccountBase> Accounts { get; } = AccountManager._accounts;

        public static readonly IDictionary<SocialService, IDictionary<long, AccountBase>> _accountsImpl = new Dictionary<SocialService, IDictionary<long, AccountBase>>();

        public static void InitializeAccounts(IEnumerable<AccountItem> accounts)
        {
            foreach (var account in accounts.Distinct().Select(a => AccountBase.FromSetting(a)))
            {
                AccountManager.Add(account);
            }
        }

        public static AccountBase Get(SocialService service, long userId)
        {
            if (_accountsImpl.TryGetValue(service, out var dic))
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

            AccountManager.GetAccountList(account.Service)
                .Add(account.Id, account);

            AccountManager._accounts.Add(account);
        }

        public static bool Contains(SocialService service, long id)
        {
            return AccountManager.GetAccountList(service).ContainsKey(id);
        }

        public static bool Contains(AccountBase account)
        {
            return account != null && AccountManager.Contains(account.Service, account.Id);
        }

        public static void Remove(AccountBase account)
        {
            if (AccountManager._accountsImpl.TryGetValue(account.Service, out var dic))
            {
                if (dic.ContainsKey(account.Id))
                {
                    dic.Remove(account.Id);
                    AccountManager._accounts.Remove(account);
                }
            }
        }

        public static void Move(int oldIndex, int newIndex)
        {
            AccountManager._accounts.Move(oldIndex, newIndex);
        }

        public static int IndexOf(AccountBase account)
        {
            return AccountManager._accounts.IndexOf(account);
        }

        public static int Count => AccountManager._accounts.Count;

        private static IDictionary<long, AccountBase> GetAccountList(SocialService service)
        {
            IDictionary<long, AccountBase> dic;

            if (_accountsImpl.TryGetValue(service, out dic))
            {
                return dic;
            }
            else
            {
                dic = new Dictionary<long, AccountBase>();
                _accountsImpl.Add(service, dic);
                return dic;
            }
        }
    }
}
