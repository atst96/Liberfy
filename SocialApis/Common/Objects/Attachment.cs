using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Common
{
    public class Attachment
    {
        public long Id { get; }
        public AttachmentItemType Type { get; }
        public string Url { get; }
        public string OriginalUrl { get; }
        public string PreviewUrl { get; }
        public string Description { get; }

        public Attachment(Twitter.MediaEntity media)
        {
            this.Id = media.Id;
            this.Url = media.Url;
            this.PreviewUrl = media.MediaUrl;
            this.OriginalUrl = media.MediaUrl;
            this.Description = media.Unwound?.Description;

            switch (media.Type)
            {
                case Twitter.MediaType.Photo:
                    this.Type = AttachmentItemType.Photo;
                    break;

                case Twitter.MediaType.AnimatedGif:
                    this.Type = AttachmentItemType.Gif;
                    break;

                case Twitter.MediaType.Video:
                    this.Type = AttachmentItemType.Video;
                    break;
            }
        }

        public Attachment(Mastodon.Attachment attachment)
        {
            this.Id = attachment.Id;
            this.Url = attachment.PreviewUrl;
            this.PreviewUrl = attachment.PreviewUrl;
            this.Description = attachment.Description;

            switch (attachment.Type)
            {
                case Mastodon.AttachmentType.Image:
                    this.Type = AttachmentItemType.Photo;
                    break;

                case Mastodon.AttachmentType.GifVideo:
                    this.Type = AttachmentItemType.Gif;
                    break;

                case Mastodon.AttachmentType.Video:
                    this.Type = AttachmentItemType.Video;
                    break;
            }
        }
    }

    public enum AttachmentItemType
    {
        Photo,
        Gif,
        Video,
    }
}
