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

            await this._api.Statuses.Post(query).ConfigureAwait(false);
        }

        private void UpdateStatusReaction(StatusItem item, Status status)
        {
            this._account.DataStore.RegisterStatus(status);

            item.Reaction.Set(isFavorited: status.Favourited, isRetweeted: status.Reblogged);
        }

        public async Task Favorite(StatusItem item)
        {
            var status = await this._api.Statuses.Favourite(item.Id).ConfigureAwait(false);

            this.UpdateStatusReaction(item, status);
        }

        public async Task Unfavorite(StatusItem item)
        {
            var status = await this._api.Statuses.Unfavourite(item.Id).ConfigureAwait(false);

            this.UpdateStatusReaction(item, status);
        }

        public async Task Retweet(StatusItem item)
        {
            var status = await this._api.Statuses.Reblog(item.Id).ConfigureAwait(false);

            this.UpdateStatusReaction(item, status);
        }

        public async Task Unretweet(StatusItem item)
        {
            var status = await this._api.Statuses.Unreblog(item.Id).ConfigureAwait(false);

            this.UpdateStatusReaction(item, status);
        }
    }
}
