using System;
using System.Collections.Concurrent;
using System.Windows.Data;
using Liberfy.Data.Mastodon;
using Liberfy.Settings;
using SocialApis.Mastodon;

namespace Liberfy.Managers
{
    /// <summary>
    /// 
    /// </summary>
    internal class MastodonDataManager
    {
        /// <summary>
        /// ホスト名
        /// </summary>
        public Uri Host { get; }

        /// <summary>
        /// アカウント情報
        /// </summary>
        public ConcurrentDictionary<long, AccountDetail> Accounts { get; } = new();

        /// <summary>
        /// トゥート情報
        /// </summary>
        public ConcurrentDictionary<long, TootDetail> Toots { get; } = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="host">ホスト名</param>
        public MastodonDataManager(Uri host)
        {
            this.Host = host;
            BindingOperations.EnableCollectionSynchronization(this.Accounts, new object());
            BindingOperations.EnableCollectionSynchronization(this.Toots, new object());
        }

        /// <summary>
        /// アカウント情報を取得する。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public AccountDetail GetAccount(MastodonAccountItem item)
        {
            return this.Accounts.GetOrAdd(item.Id, id => new AccountDetail(this.Host, item));
        }

        /// <summary>
        /// アカウント情報を登録する。
        /// </summary>
        /// <param name="account">アカウント情報</param>
        /// <returns></returns>
        public AccountDetail RegisterAccount(Account account)
        {
            long id = account?.Id ?? throw new ArgumentException(null, nameof(account));

            return this.Accounts.AddOrUpdate(id,
                (_) => new AccountDetail(this.Host, account),
                (_, info) => info.Update(account));
        }

        /// <summary>
        /// トゥート情報を登録する。
        /// </summary>
        /// <param name="status">トゥート情報</param>
        /// <returns></returns>
        public TootDetail RegisterStatus(Status status)
        {
            long id = status?.Id ?? throw new ArgumentException(null, nameof(status));

            return this.Toots.AddOrUpdate(id,
                (_) => new TootDetail(status, this),
                (_, info) => info.Update(status));
        }
    }
}
