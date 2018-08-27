using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Model
{
    public abstract class EntityBase
    {
        public EntityBase(string text, int indexStart, int length)
        {
            this.DisplayText = text;
            this.IndexStart = indexStart;
            this.Length = length;
        }

        public EntityBase(string text, SocialApis.Twitter.EntityBase entity)
            : this(text, entity.IndexStart, entity.IndexEnd - entity.IndexStart)
        {
        }

        public int IndexStart { get; }
        public int Length { get; }
        public int ActualIndexStart { get; set; }
        public int ActualLength { get; set; }
        public string DisplayText { get; set; }
    }

    public class HashtagEntity : EntityBase
    {
        public string Text { get; }

        public HashtagEntity(string text, SocialApis.Twitter.HashtagEntity hashtag)
            : base(text, hashtag)
        {
            this.Text = hashtag.Text;
        }
    }

    public class UrlEntity : EntityBase
    {
        public string Url { get; }
        public string DisplayUrl { get; }

        public UrlEntity(SocialApis.Twitter.UrlEntity url)
            : base(url.DisplayUrl, url)
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

        public MentionEntity(string text, SocialApis.Twitter.UserMentionEntity user)
            : base(text, user)
        {
            this.Id = user.Id;
            this.DisplayName = user.Name;
            this.UserName = user.ScreenName;
        }
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
    }

    public class PlainTextEntity : EntityBase
    {
        public PlainTextEntity(string text, int indexStart, int length)
            : base(text, indexStart, length)
        {
        }

        //public PlainTextEntity(string text)
        //    : base(text, 0, text.Length)
        //{
        //}
    }
}
