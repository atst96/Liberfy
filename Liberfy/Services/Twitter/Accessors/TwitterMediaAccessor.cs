using System.Collections.Generic;
using System.Threading.Tasks;
using Liberfy.Extensions;
using Liberfy.ViewModels;
using SocialApis;

namespace Liberfy.Services.Twitter.Accessors
{
    /// <summary>
    /// メディア操作関連のアクセサ
    /// </summary>
    internal class TwitterMediaAccessor
    {
        private TwitterAccount _account;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="account"></param>
        public TwitterMediaAccessor(TwitterAccount account)
        {
            this._account = account;
        }

        /// <summary>
        /// メディアをアップロードする。
        /// </summary>
        /// <param name="attachment">メディア情報</param>
        /// <param name="account">アカウント情報</param>
        /// <returns>MediaID</returns>
        public Task<long> Upload(UploadMedia attachment)
        {
            var mediaApi = this._account.Api.Media;
            using var stream = attachment.GetDataStream();

            bool isVideoUpload = attachment.MediaType.HasFlag(MediaType.Video);
            var uploadMediaType = isVideoUpload ? MimeTypes.Video.Mp4 : MimeTypes.OctetStream;

            var task = isVideoUpload
                ? mediaApi.ChunkedUpload(stream, uploadMediaType, null, attachment)
                : mediaApi.Upload(stream, null, attachment);

            return task.ContinueWithRan(m => m.MediaId);
        }

        /// <summary>
        /// 複数のメディアをアップロードする。
        /// </summary>
        /// <param name="attachments">メディア</param>
        /// <param name="account">アカウント情報</param>
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
