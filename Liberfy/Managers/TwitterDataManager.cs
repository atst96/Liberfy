using System;
using System.Collections.Concurrent;
using System.Windows.Data;
using Liberfy.Data.Twitter;
using Liberfy.Settings;
using SocialApis.Twitter;

namespace Liberfy.Managers
{
    /// <summary>
    /// 
    /// </summary>
    internal class TwitterDataManager
    {
        /// <summary>
        /// アカウント情報
        /// </summary>
        public ConcurrentDictionary<long, UserDetail> Accounts { get; } = new();

        /// <summary>
        /// トゥート情報
        /// </summary>
        public ConcurrentDictionary<long, TweetDetail> Tweets { get; } = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TwitterDataManager()
        {
            BindingOperations.EnableCollectionSynchronization(this.Accounts, new object());
            BindingOperations.EnableCollectionSynchronization(this.Tweets, new object());
        }

        /// <summary>
        /// アカウント設定からアカウント情報を生成する。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public UserDetail GetAccount(TwitterAccountSetting item)
        {
            return this.Accounts.GetOrAdd(item.UserId, id => new UserDetail(item));
        }

        /// <summary>
        /// アカウント情報を登録する。
        /// </summary>
        /// <param name="account">アカウント情報</param>
        /// <returns></returns>
        public UserDetail RegisterAccount(User account)
        {
            long id = account.Id;

            return this.Accounts.AddOrUpdate(id,
                (_) => new UserDetail(account),
                (_, info) => info.Update(account));
        }

        /// <summary>
        /// ツイート情報を登録する。
        /// </summary>
        /// <param name="status">ツイート情報</param>
        /// <returns></returns>
        public TweetDetail RegisterStatus(Status status)
        {
            long id = status?.Id ?? throw new ArgumentException(null, nameof(status));

            return this.Tweets.AddOrUpdate(id,
                (_) => new TweetDetail(status, this),
                (_, info) => info.Update(status));
        }
    }
}
