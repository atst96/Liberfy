using Liberfy.Services.Common;
using Liberfy.ViewModels;
using SocialApis;
using SocialApis.Mastodon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Services.Mastodon
{
    internal class MastodonApiGateway : IApiGateway
    {
        private readonly MastodonAccount _account;
        private readonly MastodonApi _api;

        public MastodonApiGateway(MastodonAccount account)
        {
            this._account = account;
            this._api = account.Tokens;
        }

        private async Task<long> UploadAttachment(UploadMedia attachment)
        {
            attachment.SourceStream.Position = 0;

            var media = await this._api.Media.Upload(attachment.SourceStream, attachment.Description, progress: attachment);

            return media.Id;
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

            //if (parameters.ReplyToStatus != null)
            //{
            //    query["in_reply_to_id"] = parameters.ReplyToStatus.Id;
            //}

            if (parameters.Attachments.HasItems)
            {
                query["media_ids"] = await this.UploadAttachments(parameters.Attachments).ConfigureAwait(false);
            }

            if (parameters.IsContainsWarningAttachment)
            {
                query["sensitive"] = true;
            }

            if (!string.IsNullOrEmpty(parameters.SpoilerText))
            {
                query["spoiler_text"] = parameters.SpoilerText;
            }

            //if (parameters.Visibility != null)
            //{
            //    query["visibility"] = GetVisibilityValue(parameters.Visibility);
            //}

            await this._api.Statuses.Post(query).ConfigureAwait(false);
        }

        public async Task Favorite(StatusItem item)
        {
            if (!item.Account.Equals(this._account))
            {
                return;
            }

            var task = item.Reaction.IsFavorited
                ? this._api.Statuses.Unfavourite(item.Id)
                : this._api.Statuses.Favourite(item.Id);

            var status = await task.ConfigureAwait(false);

            var statusInfo = this._account.DataStore.RegisterStatus(status);

            item.Reaction.IsFavorited = status.Favourited ?? !item.Reaction.IsFavorited;
        }

        public async Task Retweet(StatusItem item)
        {
            if (!item.Account.Equals(this._account))
            {
                return;
            }

            var task = item.Reaction.IsRetweeted
                ? this._api.Statuses.Unreblog(item.Id)
                : this._api.Statuses.Reblog(item.Id);

            var status = await task.ConfigureAwait(false);

            var statusInfo = this._account.DataStore.RegisterStatus(status);

            item.Reaction.IsRetweeted = status.Reblogged ?? !item.Reaction.IsRetweeted;
        }
    }
}
