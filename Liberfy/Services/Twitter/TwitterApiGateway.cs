using Liberfy.Services.Common;
using Liberfy.ViewModel;
using SocialApis;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Services.Twitter
{
    internal class TwitterApiGateway : IApiGateway
    {
        private readonly TwitterApi _api;

        public TwitterApiGateway(TwitterApi api)
        {
            this._api = api;
        }

        private async Task<long> UploadAttachment(UploadMedia attachment)
        {
            bool isVideoUpload = attachment.MediaType.HasFlag(ViewModel.MediaType.Video);
            var uploadMediaType = isVideoUpload ? MimeTypes.Video.Mp4 : MimeTypes.OctetStream;

            Task<MediaResponse> task;

            if (isVideoUpload)
            {
                task = this._api.Media.ChunkedUpload(
                    media: attachment.SourceStream,
                    mediaType: uploadMediaType);
            }
            else
            {
                task = this._api.Media.Upload(attachment.SourceStream);
            }

            var result = await task.ConfigureAwait(false);

            return result.MediaId;
        }

        private async Task<long[]> UploadAttachments(ICollection<UploadMedia> attachments)
        {
            var tasks = attachments.Select(m => this.UploadAttachment(m));

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return tasks.Select(t => t.Result).ToArray();
        }

        public async Task PostStatus(ServicePostParameters parameters)
        {
            var query = new Query
            {
                ["status"] = parameters.Text,
            };

            if (parameters.Attachments.HasItems)
            {
                query["media_ids"] = await this.UploadAttachments(parameters.Attachments).ConfigureAwait(false);
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
    }
}
