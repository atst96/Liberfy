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
    public class Status
    {
        [DataMember(Name = "id")]
        [JsonFormatter(typeof(Formatters.StringToLongFormatter))]
        public long Id { get; private set; }

        [DataMember(Name = "uri")]
        public string Uri { get; private set; }

        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "account")]
        public Account Account { get; private set; }

        [DataMember(Name = "in_reply_to_id")]
        [JsonFormatter(typeof(Formatters.StringToNullableLongFormatter))]
        public long? InReplyToId { get; private set; }

        [DataMember(Name = "in_reply_to_account_id")]
        [JsonFormatter(typeof(Formatters.StringToNullableLongFormatter))]
        public long? InReplyToAccountId { get; private set; }

        [DataMember(Name = "reblog")]
        public Status Reblog { get; private set; }

        [DataMember(Name = "content")]
        public string Content { get; private set; }

        [DataMember(Name = "created_at")]
        public DateTimeOffset CreatedAt { get; private set; }

        [DataMember(Name = "emojis")]
        public Emoji[] Emojis { get; private set; }

        [DataMember(Name = "replies_count")]
        public int RepliesCount { get; private set; }

        [DataMember(Name = "reblogs_count")]
        public int ReblogsCount { get; private set; }

        [DataMember(Name = "favourites_count")]
        public int FavouritesCount { get; private set; }

        [DataMember(Name = "reblogged")]
        public bool? Reblogged { get; private set; }

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

        [DataMember(Name = "card")]
        public Card Card { get; private set; }

        [DataMember(Name = "poll")]
        public Poll Poll { get; private set; }

        [DataMember(Name = "application")]
        public Application Application { get; private set; }

        [DataMember(Name = "language")]
        public string Language { get; private set; }

        [DataMember(Name = "pinned")]
        public bool? Pinned { get; private set; }
    }
}
