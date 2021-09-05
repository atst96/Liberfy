using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Liberfy.Services.Mastodon;
using Liberfy.Services.Twitter;
using Liberfy.Settings;
using SocialApis.Mastodon;
using SocialApis.Twitter;

namespace Liberfy.Managers
{
    /// <summary>
    /// アカウント管理クラス
    /// </summary>
    internal class AccountManager : ICollection<IAccount>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// コレクション変更通知イベントハンドラ
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// プロパティ変更通知イベントハンドラ
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// アカウント登録通知イベントハンドラ
        /// </summary>
        public event EventHandler<IAccount> Registered;

        /// <summary>
        /// アカウント削除通知日ベントハンドラ
        /// </summary>
        public event EventHandler<IAccount> Removed;

        public int Count => this._account.Count;

        bool ICollection<IAccount>.IsReadOnly { get; } = false;

        /// <summary>
        /// アカウントリスト
        /// </summary>
        private readonly List<IAccount> _account = new();

        private static readonly Random _random = new();

        public AccountManager()
        {
        }

        /// <summary>
        /// 一意なアカウント識別IDを生成する。
        /// </summary>
        /// <returns>アカウント識別ID</returns>
        public string GenerateUniqueId()
        {
            var registeredIds = this.Select(a => a.ItemId).ToImmutableHashSet();

            string newId;

            do
            {
                newId = _random.Next(short.MaxValue, int.MaxValue).ToString("x");
            }
            while (registeredIds.Contains(newId));

            return newId;
        }

        /// <summary>
        /// Twitterアカウントを登録する。
        /// </summary>
        /// <param name="api"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        public TwitterAccount RegisterOrUpdate(TwitterApi api, User userStatus)
        {
            if (this.GetTwitter(api.UserId, out var registeredAccount))
            {
                registeredAccount.SetApiTokens(api);
            }
            else
            {
                registeredAccount = new TwitterAccount(this.GenerateUniqueId(), api, userStatus);
                this.Add(registeredAccount);
            }

            return registeredAccount;
        }

        /// <summary>
        /// Mastodonアカウントを登録する。
        /// </summary>
        /// <param name="api"></param>
        /// <param name="accountStatus"></param>
        /// <returns></returns>
        public MastodonAccount RegisterOrUpdate(MastodonApi api, Account accountStatus)
        {
            if (this.GetMastodon(api.HostUrl, accountStatus.Id, out var registeredAccount))
            {
                registeredAccount.UpdateApiTokens(api);
            }
            else
            {
                registeredAccount = new MastodonAccount(this.GenerateUniqueId(), api, accountStatus);
                this.Add(registeredAccount);
            }

            return registeredAccount;
        }

        /// <summary>
        /// 登録済みのTwitterアカウントを取得する。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool GetTwitter(long id, out TwitterAccount account)
        {
            account = this.OfType<TwitterAccount>().FirstOrDefault(a => a.Info.Id == id);
            return account != null;
        }

        /// <summary>
        /// 登録済みのMastodonアカウントを取得する。
        /// </summary>
        /// <param name="host"></param>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool GetMastodon(Uri host, long id, out MastodonAccount account)
        {
            account = this.OfType<MastodonAccount>()
                .FirstOrDefault(u => u.Id == id && string.Equals(u.InstanceHost, host.Host, StringComparison.OrdinalIgnoreCase));

            return account != null;
        }

        /// <summary>
        /// <see cref="IAccountAuthenticator"/>からアカウントを登録する。
        /// TODO: 処理を別の場所に移す
        /// </summary>
        /// <param name="authenticator"></param>
        /// <returns></returns>
        [Obsolete]
        public async Task RegisterFromAuthenticator(IAccountAuthenticator authenticator)
        {
            if (authenticator == null)
            {
                throw new ArgumentNullException(nameof(authenticator));
            }

            var dispatcher = Dispatcher.CurrentDispatcher;

            if (authenticator is TwitterAccountAuthenticator twitterAuth)
            {
                var api = (TwitterApi)twitterAuth.Api;
                var user = await api.Account.VerifyCredentials().ConfigureAwait(false);
                var account = dispatcher.Invoke(() => this.RegisterOrUpdate(api, user));

                await account.StartActivity().ConfigureAwait(false);
            }
            else if (authenticator is MastodonAccountAuthenticator mastodonAuth)
            {
                var api = (MastodonApi)mastodonAuth.Api;
                var user = await api.Accounts.VerifyCredentials().ConfigureAwait(false);
                var account = dispatcher.Invoke(() => this.RegisterOrUpdate(api, user));

                await account.StartActivity().ConfigureAwait(false);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// アカウントを追加する
        /// </summary>
        /// <param name="item"></param>
        public void Add(IAccount item)
        {
            this._account.Add(item);
            this.Registered?.Invoke(this, item);
            this.OnCollectionChanged(new(NotifyCollectionChangedAction.Add, changedItem: item));
            this.OnPropertyChanged(nameof(this.Count));
        }

        /// <summary>
        /// アカウント情報を全消去する。
        /// </summary>
        public void Clear()
        {
            var removingAccounts = this._account.ToArray();
            this._account.Clear();

            this.OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
            this.OnPropertyChanged(nameof(this.Count));

            if (this.Removed != null)
            {
                foreach (var account in removingAccounts)
                {
                    this.Removed(this, account);
                }
            }
        }

        bool ICollection<IAccount>.Contains(IAccount item)
        {
            return this._account.Contains(item, EqualityComparer<IAccount>.Default);
        }

        void ICollection<IAccount>.CopyTo(IAccount[] array, int arrayIndex)
        {
            this._account.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// アカウントを削除する。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(IAccount item)
        {
            if (this._account.Remove(item))
            {
                this.OnCollectionChanged(new(NotifyCollectionChangedAction.Remove, (object)item));
                this.OnPropertyChanged(nameof(this.Count));
                this.Removed?.Invoke(this, item);

                return true;
            }

            return false;
        }

        IEnumerator<IAccount> IEnumerable<IAccount>.GetEnumerator()
        {
            return this._account.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return this._account.GetEnumerator();
        }

        /// <summary>
        /// アカウント情報を設定から復元する。
        /// </summary>
        /// <param name="targetAccounts"></param>
        public void Restore(IEnumerable<IAccountSetting> targetAccounts)
        {
            if (targetAccounts == null || !targetAccounts.Any())
            {
                return;
            }

            // リストア処理を見直す
            var accounts = targetAccounts
                .Select(a => AccountBase.FromSetting(a))
                .ToArray();

            if (accounts.Any())
            {
                this._account.AddRange(accounts);

                this.OnCollectionChanged(new(NotifyCollectionChangedAction.Add, changedItems: accounts));
                this.OnPropertyChanged(nameof(this.Count));
            }
        }

        /// <summary>
        /// デフォルトアカウントを取得する
        /// </summary>
        /// <returns></returns>
        public IAccount GetDefault() => this.FirstOrDefault();

        /// <summary>
        /// コレクションの変更通知
        /// </summary>
        /// <param name="args"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            this.CollectionChanged?.Invoke(this, args);
        }

        /// <summary>
        /// プロパティの変更通知
        /// </summary>
        /// <param name="propertyName"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
