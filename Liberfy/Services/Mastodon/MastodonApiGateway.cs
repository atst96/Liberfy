using Liberfy.Services.Common;
using Liberfy.ViewModel;
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
		private readonly MastodonApi _api;

		public MastodonApiGateway(MastodonApi api)
		{
			this._api = api;
		}

		private async Task<long> UploadAttachment(UploadMedia attachment)
		{
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
				query["media_ids"] = new QueryArrayItem(await this.UploadAttachments(parameters.Attachments).ConfigureAwait(false));
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
	}
}
