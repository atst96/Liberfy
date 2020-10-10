using System.Linq;
using System.Threading.Tasks;
using Liberfy.Extensions;
using SocialApis;

namespace Liberfy.Services.Mastodon.Accessors
{
    /// <summary>
    /// トゥート操作に関するアクセサ
    /// </summary>
    internal class MastodonStatusAccessor
    {
        /// <summary>
        /// アカウント情報
        /// </summary>
        private MastodonAccount _account;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="account">アカウント情報</param>
        public MastodonStatusAccessor(MastodonAccount account)
        {
            this._account = account;
        }

        /// <summary>
        /// トゥートする。
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="account">アカウント情報</param>
        /// <returns></returns>
        public async Task Toat(ServicePostParameters parameters)
        {
            var query = new Query
            {
                ["status"] = parameters.Text,
            };

            //if (parameters.ReplyToStatus != null)
            //{
            //    query["in_reply_to_id"] = parameters.ReplyToStatus.Id;
            //}

            if (parameters.Attachments.HasItems)
            {
                query["media_ids"] = await this._account.Media.Uploads(parameters.Attachments)
                    .ConfigureAwait(false);
            }

            if (parameters.IsContainsWarningAttachment)
            {
                query["sensitive"] = true;
            }

            if (!string.IsNullOrEmpty(parameters.SpoilerText))
            {
                query["spoiler_text"] = parameters.SpoilerText;
            }

            if (parameters.HasPolls)
            {
                var polls = parameters.Polls
                    .Where(poll => !string.IsNullOrEmpty(poll.Text))
                    .Select(poll => poll.Text);

                if (polls.Any())
                {
                    var pollsQuery = new Query
                    {
                        ["options"] = polls,
                        ["expires_in"] = parameters.PollsExpires,
                        ["multiple"] = parameters.IsPollsMultiple,
                        ["hide_totals"] = parameters.IsPollsHideTotals,
                    };

                    query["poll"] = pollsQuery;
                }
            }

            //if (parameters.Visibility != null)
            //{
            //    query["visibility"] = GetVisibilityValue(parameters.Visibility);
            //}

            await this._account.Api.Statuses.Post(query).ConfigureAwait(false);
        }

        /// <summary>
        /// トゥートに対して「お気に入り」を行う。
        /// </summary>
        /// <param name="status">「お気に入り」を行うトゥート</param>
        public Task Favorite(StatusItem status)
        {
            return this._account.Api.Statuses.Favourite(status.Id)
                .ContinueWithRan(s => status.Reaction.Set(isFavorited: s.Favourited ?? true));
        }

        /// <summary>
        /// トゥートに対しての「お気に入り」を解除する。
        /// </summary>
        /// <param name="status">「お気に入り」を解除するトゥート</param>
        public Task Unfavorite(StatusItem status)
        {
            return this._account.Api.Statuses.Unfavourite(status.Id)
                .ContinueWithRan(s => status.Reaction.Set(isFavorited: s.Favourited ?? false));
        }

        /// <summary>
        /// トゥートをブーストする。
        /// </summary>
        /// <param name="status">ブースト対象のトゥート</param>
        public Task Reblog(StatusItem status)
        {
            return this._account.Api.Statuses.Reblog(status.Id)
                .ContinueWithRan(s => status.Reaction.Set(isRetweeted: s.Reblogged ?? true));
        }

        /// <summary>
        /// トゥートに対するブーストを削除する。
        /// </summary>
        /// <param name="status">削除するブースト または ブーストを解除するトゥート</param>
        public Task Unreblog(StatusItem status)
        {
            return this._account.Api.Statuses.Unreblog(status.Id)
                .ContinueWithRan(s => status.Reaction.Set(isRetweeted: s.Reblogged ?? false));
        }
    }
}
