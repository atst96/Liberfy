using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Model
{
    internal interface IEntity
    {
        string DisplayText { get; set; }
    }

    public class HashtagEntity : IEntity
    {
        public string Text { get; }
        public string DisplayText { get; set; }

        public HashtagEntity(string text)
        {
            this.Text = text;
            this.DisplayText = text;
        }

        public HashtagEntity(string text, SocialApis.Twitter.HashtagEntity hashtag) 
        {
            this.Text = hashtag.Text;
            this.DisplayText = text;
        }
    }

    public class UrlEntity : IEntity
    {
        public string Url { get; }
        public string DisplayText { get; set; }

        public UrlEntity(string url, string displayUrl)
        {
            this.Url = url;
            this.DisplayText = displayUrl;
        }

        public UrlEntity(SocialApis.Twitter.UrlEntity url)
            : this(url.Url, url.DisplayUrl)
        {
        }
    }

    public class MentionEntity : IEntity
    {
        public long Id { get; }
        public string DisplayName { get; }
        public string UserName { get; }
        public string DisplayText { get; set; }

        public MentionEntity(string text, SocialApis.Twitter.UserMentionEntity user)
        {
            this.Id = user.Id;
            this.DisplayName = user.Name;
            this.UserName = user.ScreenName;
            this.DisplayText = text;
        }

        public MentionEntity(string displayName, string screenName)
        {
            this.DisplayName = displayName;
            this.UserName = screenName;
            this.DisplayText = displayName;
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

    public class PlainTextEntity : IEntity
    {
        public string DisplayText { get; set; }

        public PlainTextEntity(string text)
        {
            this.DisplayText = text;
        }
    }

    public class EmojiEntity : IEntity
    {
        public string DisplayText { get; set; }
        
        public string ShortCode { get; set; }

        public string ImageUrl { get; set; }

        public EmojiEntity(SocialApis.Mastodon.Emoji emoji)
        {
            this.DisplayText = $":{ emoji.ShortCode }:";
            this.ShortCode = emoji.ShortCode;
            this.ImageUrl = emoji.Url;
        }
    }
}
