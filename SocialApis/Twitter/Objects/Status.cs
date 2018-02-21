using System;
using System.Runtime.Serialization;
using Utf8Json;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Status
    {
        [DataMember(Name = "created_at")]
        [JsonFormatter(typeof(DateTimeOffsetFormatter))]
        public DateTimeOffset CreatedAt { get; private set; }

        [DataMember(Name = "id")]
        public long Id { get; private set; }

        [DataMember(Name = "text")]
        public string Text { get; private set; }

        [DataMember(Name = "source")]
        public string Source { get; private set; }

        [DataMember(Name = "truncated")]
        public bool IsTruncated { get; private set; }

        [DataMember(Name = "in_reply_to_status_id")]
        public long? InReplyToStatusId { get; private set; }

        [DataMember(Name = "in_reply_to_user_id")]
        public long? InReplyToUserId { get; private set; }

        [DataMember(Name = "in_reply_to_screen_name")]
        public string InReplyToScreenName { get; private set; }

        [DataMember(Name = "user")]
        public User User { get; private set; }

        [DataMember(Name = "coordinates")]
        public Coordinates<Point> Coordinates { get; private set; }

        [DataMember(Name = "place")]
        public Places Place { get; private set; }

        [DataMember(Name = "quoted_status_id")]
        public long? QuotedStatusId { get; private set; }

        [DataMember(Name = "is_quote_status")]
        public bool IsQuotedStatus { get; private set; }

        [DataMember(Name = "quoted_status")]
        public Status QuotedStatus { get; private set; }

        [DataMember(Name = "retweeted_status")]
        public Status RetweetedStatus { get; private set; }

        [DataMember(Name = "quote_count")]
        public long? QuoteCount { get; private set; }

        [DataMember(Name = "reply_count")]
        public long ReplyCount { get; private set; }

        [DataMember(Name = "retweet_count")]
        public long RetweetCount { get; private set; }

        [DataMember(Name = "favorite_count")]
        public long FavoriteCount { get; private set; }

        [DataMember(Name = "entities")]
        public Entities Entities { get; private set; }

        [DataMember(Name = "extended_entities")]
        public ExtendedEntities ExtendedEntities { get; private set; }

        [DataMember(Name = "favorited")]
        public bool IsFavorited { get; private set; }

        [DataMember(Name = "retweeted")]
        public bool Retweeted { get; private set; }

        [DataMember(Name = "possibly_sensitive")]
        public bool PossiblySensitive { get; private set; }

        [DataMember(Name = "filter_level")]
        public string FilterLevel { get; private set; }

        [DataMember(Name = "lang")]
        public string Language { get; private set; }

        // [DataMember(Name = "matching_rules")]

        // Additional tweet attributes

        [DataMember(Name = "current_user_retweet")]
        public IdObject? CurrentUserRetweet { get; private set; }

        // [DataMember(Name = "scopes")]

        [DataMember(Name = "withheld_copyright")]
        public bool IsWithheldCopyright { get; private set; }

        [DataMember(Name = "withheld_in_countries")]
        public string[] WithheldInCountries { get; private set; }

        [DataMember(Name = "withheld_scope")]
        public string WithheldScope { get; private set; }
    }
}
