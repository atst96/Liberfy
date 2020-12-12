using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liberfy.Data.Settings.Columns;
using Liberfy.Services.Mastodon;
using Liberfy.Services.Twitter;
using SocialApis.Mastodon;
using SocialApis.Twitter;

namespace Liberfy.Managers
{
    internal static class AccountManager
    {
        private static Random _random = new();

        public static NotifiableCollection<IAccount> Accounts { get; } = new();

        /// <summary>
        /// アカウント識別IDを生成する。
        /// </summary>
        /// <returns>アカウント識別ID</returns>
        public static string GenerateUniqueId()
        {
            var registeredIds = Accounts.Select(a => a.ItemId).ToImmutableHashSet();

            string newId;

            do
            {
                newId = _random.Next(short.MaxValue, int.MaxValue).ToString("x");
            }
            while (registeredIds.Contains(newId));

            return newId;
        }
        public static async Task RegisterFromAuthenticator(IAccountAuthenticator authenticator)
        {
            if (authenticator == null)
            {
                throw new ArgumentNullException(nameof(authenticator));
            }

            if (authenticator is TwitterAccountAuthenticator twitterAuth)
            {
                var api = (TwitterApi)twitterAuth.Api;

                var account = Accounts.OfType<TwitterAccount>()
                    .FirstOrDefault(a => a.Info.Id == api.UserId);

                if (account != null)
                {
                    account.SetApiTokens(api);
                }
                else
                {
                    var user = await api.Account.VerifyCredentials().ConfigureAwait(false);
                    var itemId = GenerateUniqueId();

                    account = new TwitterAccount(itemId, api, user);
                    RegisterNewAccount(account);
                }

                await account.StartActivity().ConfigureAwait(false);
            }
            else if (authenticator is MastodonAccountAuthenticator mastodonAuth)
            {
                var api = (MastodonApi)mastodonAuth.Api;
                var user = await api.Accounts.VerifyCredentials().ConfigureAwait(false);

                var account = Accounts.OfType<MastodonAccount>()
                    .FirstOrDefault(u => u.Info.Id == user.Id && string.Equals(u.InstanceHost, api.HostUrl.Host, StringComparison.OrdinalIgnoreCase));

                if (account != null)
                {
                    account.UpdateApiTokens(api);
                }
                else
                {
                    var itemId = GenerateUniqueId();
                    account = new MastodonAccount(itemId, api, user);

                    RegisterNewAccount(account);
                }

                await account.StartActivity().ConfigureAwait(false);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void RegisterNewAccount(IAccount account) => App.Current.Dispatcher.Invoke(() =>
        {
            Accounts.Add(account);

            // NOTE:
            // Home、Notificationカラムを作成する

            var defaultColumns = new IColumnSetting[]
            {
                new HomeColumnSetting(account.ItemId),
                new NotificationColumnSetting(account.ItemId),
            };

            foreach (var columnOptions in defaultColumns)
            {
                if (ColumnBase.TryFromSetting(columnOptions, account, out var column))
                {
                    TimelineBase.Columns.Add(column);
                }
            }
        });
    }
}
