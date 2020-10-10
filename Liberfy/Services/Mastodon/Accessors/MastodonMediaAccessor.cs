using System.Collections.Generic;
using System.Threading.Tasks;
using Liberfy.Extensions;
using Liberfy.ViewModels;

namespace Liberfy.Services.Mastodon.Accessors
{
    /// <summary>
    /// メディア操作に関するアクセサ
    /// </summary>
    internal class MastodonMediaAccessor
    {
        /// <summary>
        /// アカウント情報
        /// </summary>
        private MastodonAccount _account;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="account">アカウント情報</param>
        public MastodonMediaAccessor(MastodonAccount account)
        {
            this._account = account;
        }

        /// <summary>
        /// メディアをアップロードする。
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns>MediaID</returns>
        public Task<long> Upload(UploadMedia attachment)
        {
            using var sourceStream = attachment.GetDataStream();

            return this._account.Api.Media.Upload(sourceStream, attachment.Description, progress: attachment)
                .ContinueWithRan(m => m.Id);
        }

        /// <summary>
        /// 複数のメディアをアップロードする。
        /// </summary>
        /// <param name="attachments"></param>
        /// <returns>MediaID</returns>
        public Task<long[]> Uploads(ICollection<UploadMedia> attachments)
        {
            var tasks = new List<Task<long>>(attachments.Count);

            foreach (var item in attachments)
            {
                tasks.Add(this.Upload(item));
            }

            return Task.WhenAll(tasks);
        }
    }
}
