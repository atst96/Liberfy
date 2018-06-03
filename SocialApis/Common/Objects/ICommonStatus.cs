using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Common
{
    public interface ICommonStatus
    {
        SocialService Service { get; }

        long Id { get; }

        DateTimeOffset CreatedAt { get; }

        string Text { get; }

        string FilterLevel { get; }

        long? InReplyToStatusId { get; }

        long? InReplyToUserId { get; }

        string Language { get; }

        bool IsSensitive { get; }

        string SourceName { get; }

        string SourceUrl { get; }

        ICommonStatus RetweetedStatus { get; }

        bool IsQuotedStatus { get; }

        ICommonStatus QuotedStatus { get; }

        string SpoilerText { get; }

        ICommonAccount User { get; }

        int? FavoriteCount { get; }

        int? RetweetCount { get; }

        bool? IsRetweeted { get; }

        bool? IsFavorited { get; }

        Visibility Visibility { get; }

        Attachment[] Attachments { get; }

        EntityBase[] Entities { get; }
    }
}
