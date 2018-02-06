using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Status
    {
        [DataMember(Name = "created_at")]
        [JsonFormatter(typeof(DateTimeOffsetFormatter))]
        private DateTimeOffset _createdAt;
        public DateTimeOffset CreatedAt => _createdAt;

        [DataMember(Name = "id")]
        private long _id;
        public long Id => _id;

        [DataMember(Name = "text")]
        private string _text;
        public string Text => _text;

        [DataMember(Name = "source")]
        private string _source;
        public string Source => _source;

        [DataMember(Name = "truncated")]
        private bool _truncated;
        public bool IsTruncated => _truncated;

        [DataMember(Name = "in_reply_to_status_id")]
        private long? _inReplyToStatusId;
        public long? InReplyToStatusId => _inReplyToStatusId;

        [DataMember(Name = "in_reply_to_user_id")]
        private long? _inRelyToUserId;
        public long? InReplyToUserId => _inRelyToUserId;

        [DataMember(Name = "in_reply_to_screen_name")]
        private string _inReplyToScreenName;
        public string InReplyToScreenName => _inReplyToScreenName;

        [DataMember(Name = "user")]
        private User _user;
        public User User => _user;

        [DataMember(Name = "coordinates")]
        private Coordinates _coordinates;
        public Coordinates Coordinates => _coordinates;

        [DataMember(Name = "place")]
        private Places _place;
        public Places Place => _place;

        [DataMember(Name = "quoted_status_id")]
        private long? _quotedStatusId;
        public long? QuotedStatusId => _quotedStatusId;

        [DataMember(Name = "is_quote_status")]
        private bool _isQuotedStatus;
        public bool IsQuotedStatus => _isQuotedStatus;

        [DataMember(Name = "quoted_status")]
        private Status _quotedStatus;
        public Status QuotedStatus => _quotedStatus;

        [DataMember(Name = "retweeted_status")]
        private Status _retweetedStatus;
        public Status RetweetedStatus => _retweetedStatus;

        [DataMember(Name = "quote_count")]
        private long? _quoteCount;
        public long? QuoteCount => _quoteCount;

        [DataMember(Name = "reply_count")]
        private long _replyCount;
        public long ReplyCount => _replyCount;

        [DataMember(Name = "retweet_count")]
        private long _retweetCount;
        public long RetweetCount => _retweetCount;

        [DataMember(Name = "favorite_count")]
        private long _favoriteCount;
        public long FavoriteCount => _favoriteCount;

        [DataMember(Name = "entities")]
        private Entities _entities;
        public Entities Entities => _entities;

        [DataMember(Name = "extended_entities")]
        private ExtendedEntities _extendedEntities;
        public ExtendedEntities ExtendedEntities => _extendedEntities;

        [DataMember(Name = "favorited")]
        private bool _favorited;
        public bool IsFavorited => _favorited;

        [DataMember(Name = "retweeted")]
        private bool _retweeted;
        public bool Retweeted => _retweeted;

        [DataMember(Name = "possibly_sensitive")]
        private bool _possiblySensitive;
        public bool PossiblySensitive => _possiblySensitive;

        [DataMember(Name = "filter_level")]
        private string _filterLevel;
        public string FilterLevel => _filterLevel;

        [DataMember(Name = "lang")]
        private string _lang;
        public string Language => _lang;

        // [DataMember(Name = "matching_rules")]

        // Additional tweet attributes

        [DataMember(Name = "current_user_retweet")]
        private IdObject _currentUserRetweet { get; }
        public long? CurrentUserRetweet => _currentUserRetweet?.Id;

        // [DataMember(Name = "scopes")]

        [DataMember(Name = "withheld_copyright")]
        private bool _withheldCopyright;
        public bool IsWithheldCopyright => _withheldCopyright;

        [DataMember(Name = "withheld_in_countries")]
        private string[] _withheldInCountires;
        public string[] WithheldInCountries => _withheldInCountires;

        [DataMember(Name = "withheld_scope")]
        private string _withheldScope;
        public string WithheldScope => _withheldScope;
    }
}
