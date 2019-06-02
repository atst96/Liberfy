using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterApi = SocialApis.Twitter;
using MastodonApi = SocialApis.Mastodon;

namespace Liberfy.Model
{
    internal class Attachment
    {
        public long Id { get; }
        public AttachmentType Type { get; }
        public string Url { get; }
        public string OriginalUrl { get; }
        public string PreviewUrl { get; }
        public string Description { get; }

        public Attachment(TwitterApi.MediaEntity media)
        {
            this.Id = media.Id;
            this.Url = media.Url;
            this.PreviewUrl = media.MediaUrl;
            this.OriginalUrl = media.MediaUrl;
            this.Description = media.Unwound?.Description;
            this.Type = TwitterValueConverter.ToAttachmentType(media.Type);
        }

        public Attachment(MastodonApi.Attachment attachment)
        {
            this.Id = attachment.Id;
            this.Url = attachment.PreviewUrl;
            this.PreviewUrl = attachment.PreviewUrl;
            this.Description = attachment.Description;
            this.Type = MastodonValueConverter.ToAttachmentType(attachment.Type);
        }
    }

    public enum AttachmentType
    {
        Unknown,
        Photo,
        Gif,
        Video,
    }
}
