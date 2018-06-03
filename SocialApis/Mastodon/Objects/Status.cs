using SocialApis.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Status : ICommonStatus
    {
        [DataMember(Name = "id")]
        [JsonFormatter(typeof(Formatters.ToLongFormatter))]
        public long Id { get; private set; }

        [DataMember(Name = "uri")]
        public string Uri { get; private set; }

        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "account")]
        public Account Account { get; private set; }

        [DataMember(Name = "in_reply_to_id")]
        [JsonFormatter(typeof(Formatters.ToNullableLongFormatter))]
        public long? InReplyToId { get; private set; }

        [DataMember(Name = "in_reply_to_account_id")]
        [JsonFormatter(typeof(Formatters.ToNullableLongFormatter))]
        public long? InReplyToStatusId { get; private set; }

        [DataMember(Name = "reblog")]
        public Status Reblog { get; private set; }

        [DataMember(Name = "content")]
        public string Content { get; private set; }

        [DataMember(Name = "created_at")]
        public DateTimeOffset CreatedAt { get; private set; }

        [DataMember(Name = "emojis")]
        public Emoji[] Emojis { get; private set; }

        [DataMember(Name = "reblogs_count")]
        public int ReblogsCount { get; private set; }

        [DataMember(Name = "favorites_count")]
        public int FavouritesCount { get; private set; }

        [DataMember(Name = "reblogged")]
        public bool? Rebloged { get; private set; }

        [DataMember(Name = "favourited")]
        public bool? Favourited { get; private set; }

        [DataMember(Name = "muted")]
        public bool? Muted { get; private set; }

        [DataMember(Name = "sensitive")]
        public bool Sensitive { get; private set; }

        [DataMember(Name = "spoiler_text")]
        public string SpoilerText { get; private set; }

        [DataMember(Name = "visibility")]
        public StatusVisibility Visibility { get; private set; }

        [DataMember(Name = "media_attachments")]
        public Attachment[] MediaAttachments { get; private set; }

        [DataMember(Name = "mentions")]
        public Mention[] Mentions { get; private set; }

        [DataMember(Name = "tags")]
        public Tag[] Tags { get; private set; }

        [DataMember(Name = "application")]
        public Application Application { get; private set; }

        [DataMember(Name = "language")]
        public string Language { get; private set; }

        [DataMember(Name = "pinned")]
        public bool? Pinned { get; private set; }

        [IgnoreDataMember]
        SocialService ICommonStatus.Service { get; } = SocialService.Mastodon;

        [IgnoreDataMember]
        string ICommonStatus.Text => this.Content;

        [IgnoreDataMember]
        string ICommonStatus.FilterLevel { get; }

        [IgnoreDataMember]
        long? ICommonStatus.InReplyToUserId => this.InReplyToId;

        [IgnoreDataMember]
        bool ICommonStatus.IsSensitive => this.Sensitive;

        [IgnoreDataMember]
        string ICommonStatus.SourceName => this.Application?.Name;

        [IgnoreDataMember]
        string ICommonStatus.SourceUrl => this.Application?.Website;

        [IgnoreDataMember]
        ICommonStatus ICommonStatus.RetweetedStatus => this.Reblog;

        [IgnoreDataMember]
        bool ICommonStatus.IsQuotedStatus { get; }

        [IgnoreDataMember]
        ICommonStatus ICommonStatus.QuotedStatus { get; }

        [IgnoreDataMember]
        string ICommonStatus.SpoilerText => this.SpoilerText;

        [IgnoreDataMember]
        ICommonAccount ICommonStatus.User => this.Account;

        [IgnoreDataMember]
        int? ICommonStatus.FavoriteCount => this.FavouritesCount;

        [IgnoreDataMember]
        int? ICommonStatus.RetweetCount => this.ReblogsCount;

        [IgnoreDataMember]
        bool? ICommonStatus.IsRetweeted => this.Rebloged;

        [IgnoreDataMember]
        bool? ICommonStatus.IsFavorited => this.Favourited;

        [IgnoreDataMember]
        private Common.Attachment[] _attachments;

        [IgnoreDataMember]
        Common.Attachment[] ICommonStatus.Attachments
        {
            get
            {
                if (this._attachments == null)
                {
                    this._attachments = this.MediaAttachments?
                        .Select(ma => new Common.Attachment(ma))
                        .ToArray() ?? new Common.Attachment[0];
                }

                return this._attachments;
            }
        }

        [IgnoreDataMember]
        Common.EntityBase[] ICommonStatus.Entities { get; } = new Common.EntityBase[0];

        [IgnoreDataMember]
        Common.Visibility ICommonStatus.Visibility
        {
            get
            {
                switch (this.Visibility)
                {
                    case StatusVisibility.Direct:
                        return Common.Visibility.Direct;

                    case StatusVisibility.Private:
                        return Common.Visibility.Private;

                    case StatusVisibility.Public:
                        return Common.Visibility.Public;

                    case StatusVisibility.Unlisted:
                        return Common.Visibility.Unlisted;

                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
