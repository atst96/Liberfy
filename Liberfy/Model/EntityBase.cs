using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Model
{
    public abstract class EntityBase
    {
        public EntityBase(int indexStart, int length)
        {
            this.IndexStart = indexStart;
            this.Length = length;
        }

        public EntityBase(SocialApis.Twitter.EntityBase entity)
            : this(entity.IndexStart, entity.IndexEnd - entity.IndexStart)
        {
        }

        public int IndexStart { get; }
        public int Length { get; }
        public int ActualIndexStart { get; set; }
        public int ActualLength { get; set; }
    }

    public class HashtagEntity : EntityBase
    {
        public string Text { get; }

        public HashtagEntity(SocialApis.Twitter.HashtagEntity hashtag)
            : base(hashtag)
        {
            this.Text = hashtag.Text;
        }
    }

    public class UrlEntity : EntityBase
    {
        public string Url { get; }
        public string DisplayUrl { get; }

        public UrlEntity(SocialApis.Twitter.UrlEntity url)
            : base(url)
        {
            this.Url = url.Url;
            this.DisplayUrl = url.DisplayUrl;
        }
    }

    public class MentionEntity : EntityBase
    {
        public long Id { get; }
        public string DisplayName { get; }
        public string UserName { get; }

        public MentionEntity(SocialApis.Twitter.UserMentionEntity user)
            : base(user)
        {
            this.Id = user.Id;
            this.DisplayName = user.Name;
            this.UserName = user.ScreenName;
        }

        //public MentionEntity(SocialApis.Mastodon.Mention mention)
        //    : base(mention)
        //{
        //    this.Id = mention.Id;
        //    this.DisplayName = mention.Acct;
        //    this.UserName = mention.UserName;
        //}
    }

    public class MediaEntity : UrlEntity
    {
        public long Id { get; }
        public string MediaUrl { get; }

        public MediaEntity(SocialApis.Twitter.MediaEntity media)
            : base(media)
        {
            this.Id = media.Id;
            this.MediaUrl = media.MediaUrl;
        }

        //public MediaEntity(SocialApis.Mastodon.Attachment attachment)
        //{
        //    this.Id = attachment.Id;
        //    this.MediaUrl = attachment.PreviewUrl;
        //}
    }
}
