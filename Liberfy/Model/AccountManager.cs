using Liberfy.Services.Mastodon;
using Liberfy.Services.Twitter;
using Liberfy.Settings;
using SocialApis;
using SocialApis.Mastodon;
using SocialApis.Twitter;
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

        public static IAccount Get(ServiceType service, long userId)
        {
            if (_serviceAccountMap.TryGetValue(service, out var dic))
            {
                if (dic.TryGetValue(userId, out var account))
                    return account;
            }

            return null;
        }

        private static readonly Random _random = new Random();

        /// <summary>
        /// アカウント識別IDを生成する。
        /// </summary>
        /// <returns>アカウント識別ID</returns>
        public static string GenerateUniqueId()
        {
            while (true)
            {
                int value = _random.Next(short.MaxValue, int.MaxValue);
                var id = value.ToString("x");

                if (!AccountManager.Accounts.Any(a => a.ItemId == id))
                {
                    return id;
                }
            }
        }

        public static async Task Register(IAccountAuthenticator authenticator)
        {
            if (authenticator == null)
            {
                throw new ArgumentNullException(nameof(authenticator));
            }

            IAccount account;
            bool newRegister = false;

            if (authenticator is TwitterAccountAuthenticator twitterAuth)
            {
                var api = (TwitterApi)twitterAuth.Api;

                account = AccountManager.Get(ServiceType.Twitter, api.UserId);

                if (account != null)
                {
                    account.SetApiTokens(api);
                }
                else
                {
                    var user = await api.Account.VerifyCredentials().ConfigureAwait(false);

                    account = new TwitterAccount(api, user);
                    newRegister = true;
                }
            }
            else if (authenticator is MastodonAccountAuthenticator mastodonAuth)
            {
                var api = (MastodonApi)mastodonAuth.Api;
                var user = await api.Accounts.VerifyCredentials().ConfigureAwait(false);

                account = AccountManager.Get(ServiceType.Mastodon, user.Id);

                if (account != null)
                {
                    account.SetApiTokens(api);
                }
                else
                {
                    account = new MastodonAccount(api, user);
                    newRegister = true;
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            if (newRegister)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    AccountManager.Add(account);

                    foreach (var columnOptions in App.Setting.DefaultColumns)
                    {
                        if (ColumnBase.FromSetting(columnOptions, account, out var column))
                        {
                            TimelineBase.Columns.Add(column);
                        }
                    }
                });
            }

            await account.Load().ConfigureAwait(false);
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
