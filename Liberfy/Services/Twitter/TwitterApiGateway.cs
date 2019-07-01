using Liberfy.Services.Common;
using Liberfy.ViewModels;
using SocialApis;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Services.Twitter
{
    internal class TwitterApiGateway : IApiGateway
    {
        private readonly TwitterAccount _account;
        private readonly TwitterApi _api;

        public TwitterApiGateway(TwitterAccount account)
        {
            this._account = account;
            this._api = account.Tokens;
        }

        private async Task<long> UploadAttachment(UploadMedia attachment)
        {
            using var stream = attachment.GetDataStream();

            bool isVideoUpload = attachment.MediaType.HasFlag(MediaType.Video);
            var uploadMediaType = isVideoUpload ? MimeTypes.Video.Mp4 : MimeTypes.OctetStream;

            Task<MediaResponse> task;

            if (isVideoUpload)
            {
                task = this._api.Media.ChunkedUpload(stream, uploadMediaType, null, attachment);
            }
            else
            {
                task = this._api.Media.Upload(stream, null, attachment);
            }

            var result = await task.ConfigureAwait(false);

            return result.MediaId;
        }

        private async Task<long[]> UploadAttachments(ICollection<UploadMedia> attachments)
        {
            var tasks = new List<Task<long>>(attachments.Count);

            foreach (var item in attachments)
            {
                tasks.Add(this.UploadAttachment(item));
            }

            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task PostStatus(ServicePostParameters parameters)
        {
            var query = new Query
            {
                ["status"] = parameters.Text,
            };

            if (parameters.Attachments.HasItems)
            {
                var mediaIds = await this.UploadAttachments(parameters.Attachments).ConfigureAwait(false);

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

            await this._api.Statuses.Update(query).ConfigureAwait(false);
        }

        private void UpdateStatusReaction(StatusItem item, Status status)
        {
            this._account.DataStore.RegisterStatus(status);

            item.Reaction.Set(isFavorited: status.IsFavorited, isRetweeted: status.IsRetweeted);
        }

        public async Task Favorite(StatusItem item)
        {
            var status = await this._api.Favorites.Create(item.Id).ConfigureAwait(false);

            this.UpdateStatusReaction(item, status);
        }

        public async Task Unfavorite(StatusItem item)
        {
            var status = await this._api.Favorites.Destroy(item.Id).ConfigureAwait(false);

            this.UpdateStatusReaction(item, status);
        }

        public async Task Retweet(StatusItem item)
        {
            var status = await this._api.Statuses.Retweet(item.Id).ConfigureAwait(false);

            this.UpdateStatusReaction(item, status);
        }

        public async Task Unretweet(StatusItem item)
        {
            var status = await this._api.Statuses.Destroy(item.Id).ConfigureAwait(false);

            this.UpdateStatusReaction(item, status);
        }
    }
}
