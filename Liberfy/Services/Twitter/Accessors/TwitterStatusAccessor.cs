using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liberfy.Extensions;
using SocialApis;

namespace Liberfy.Services.Twitter.Accessors
{
    /// <summary>
    /// ツイート関連操作に関するアクセサ
    /// </summary>
    internal class TwitterStatusAccessor
    {
        /// <summary>
        /// アカウント
        /// </summary>
        private TwitterAccount _account;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="account">アカウント情報</param>
        public TwitterStatusAccessor(TwitterAccount account)
        {
            this._account = account;
        }

        /// <summary>
        /// ツイートする。
        /// </summary>
        /// <param name="parameters"></param>
        public async Task Tweet(ServicePostParameters parameters)
        {
            var query = new Query
            {
                ["status"] = parameters.Text,
            };

            if (parameters.Attachments.HasItems)
            {
                var mediaIds = await this._account.Media.Uploads(parameters.Attachments)
                    .ConfigureAwait(false);

                query["media_ids"] = new ValueGroup(mediaIds);
            }

            if (parameters.IsContainsWarningAttachment)
            {
                query["possibly_sensitive"] = true;
            }

            //if (parameters.ReplyToStatus != null)
            //{
            //    query["in_reply_to_status_id"] = parameters.ReplyToStatus.Id;
            //}

            await this._account.Api.Statuses.Update(query).ConfigureAwait(false);
        }

        /// <summary>
        /// ツイートに対していいねを行う。
        /// </summary>
        /// <param name="status">いいねを行うツイート</param>
        public Task Favorite(StatusItem status)
        {
            return this._account.Api.Favorites.Create(status.Id)
                .ContinueWithRan(s => status.Reaction.Set(isFavorited: s.IsFavorited));
        }

        /// <summary>
        /// ツイートに対する いいね を解除する。
        /// </summary>
        /// <param name="status">いいねを解除するツイート</param>
        public Task Unfavorite(StatusItem status)
        {
            return this._account.Api.Favorites.Destroy(status.Id)
                .ContinueWithRan(s => status.Reaction.Set(isFavorited: s.IsFavorited));
        }

        /// <summary>
        /// ツイートをリツイートする。
        /// </summary>
        /// <param name="status">リツイート対象のツイート</param>
        public Task Retweet(StatusItem status)
        {
            return this._account.Api.Statuses.Retweet(status.Id)
                .ContinueWithRan(s => status.Reaction.Set(isRetweeted: s.IsRetweeted));
        }

        /// <summary>
        /// リツイートを削除する。
        /// </summary>
        /// <param name="status">リツイート解除対象のツイート</param>
        public Task Unretweet(StatusItem status)
        {
            return this._account.Api.Statuses.Unretweet(status.Id)
                .ContinueWithRan(s => status.Reaction.Set(isRetweeted: s.IsRetweeted));
        }
    }
}
