using SocialApis.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Utf8Json;
using System.Linq;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Status
    {
        [DataMember(Name = "created_at")]
        [JsonFormatter(typeof(DateTimeOffsetFormatter))]
        public DateTimeOffset CreatedAt { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "full_text")]
        public string FullText { get; set; }

        [DataMember(Name = "display_text_range")]
        public int[] DisplayTextRange { get; set; }

        [DataMember(Name = "contributors")]
        public long[] Contributors { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "truncated")]
        public bool IsTruncated { get; set; }

        [DataMember(Name = "in_reply_to_status_id")]
        public long? InReplyToStatusId { get; set; }

        [DataMember(Name = "in_reply_to_user_id")]
        public long? InReplyToUserId { get; set; }

        [DataMember(Name = "in_reply_to_screen_name")]
        public string InReplyToScreenName { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }

        [DataMember(Name = "coordinates")]
        public Coordinates<Point> Coordinates { get; set; }

        [DataMember(Name = "place")]
        public Places Place { get; set; }

        [DataMember(Name = "quoted_status_id")]
        public long? QuotedStatusId { get; set; }

        [DataMember(Name = "is_quote_status")]
        public bool IsQuotedStatus { get; set; }

        [DataMember(Name = "quoted_status")]
        public Status QuotedStatus { get; set; }

        [DataMember(Name = "retweeted_status")]
        public Status RetweetedStatus { get; set; }

        [DataMember(Name = "quote_count")]
        public int? QuoteCount { get; set; }

        [DataMember(Name = "reply_count")]
        public int? ReplyCount { get; set; }

        [DataMember(Name = "retweet_count")]
        public int? RetweetCount { get; set; }

        [DataMember(Name = "favorite_count")]
        public int? FavoriteCount { get; set; }

        [DataMember(Name = "entities")]
        public Entities Entities { get; set; }

        [DataMember(Name = "extended_entities")]
        public ExtendedEntities ExtendedEntities { get; set; }

        [DataMember(Name = "extended_tweet")]
        public ExtendedTweet ExtendedTweet { get; set; }

        [DataMember(Name = "favorited")]
        public bool? IsFavorited { get; set; }

        [DataMember(Name = "retweeted")]
        public bool? IsRetweeted { get; set; }

        [DataMember(Name = "possibly_sensitive")]
        public bool PossiblySensitive { get; set; }

        [DataMember(Name = "possibly_sensitive_appealable")]
        public bool PossiblySensitiveAppealable { get; set; }

        [DataMember(Name = "filter_level")]
        public string FilterLevel { get; set; }

        [DataMember(Name = "lang")]
        public string Language { get; set; }

        [DataMember(Name = "matching_rules")]
        public string MatchingRules { get; set; }

        // Additional tweet attributes

        [DataMember(Name = "current_user_retweet")]
        public IdObject? CurrentUserRetweet { get; set; }

        [DataMember(Name = "scopes")]
        public Dictionary<string, object> Scopes { get; set; }

        [DataMember(Name = "withheld_copyright")]
        public bool? IsWithheldCopyright { get; set; }

        [DataMember(Name = "withheld_in_countries")]
        public string[] WithheldInCountries { get; set; }

        [DataMember(Name = "withheld_scope")]
        public string WithheldScope { get; set; }
    }
}
