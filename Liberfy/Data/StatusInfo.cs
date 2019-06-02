using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Liberfy.Model;

using TwitterApi = SocialApis.Twitter;

namespace Liberfy
{
    internal interface IStatusInfo<T> : IStatusInfo
    {
        IStatusInfo<T> Update(T status);
    }

    internal interface IStatusInfo : INotifyPropertyChanged
    {
        ServiceType Service { get; }
        long Id { get; }
        IReadOnlyList<long> Contributors { get; }
        TwitterApi.Coordinates<TwitterApi.Point> Coordinates { get; }
        DateTimeOffset CreatedAt { get; }
        IReadOnlyList<IEntity> Entities { get; }
        IReadOnlyList<Attachment> Attachments { get; }
        string FilterLevel { get; }
        long InReplyToStatusId { get; }
        long InReplyToUserId { get; }
        string Language { get; }
        TwitterApi.Places Place { get; }
        bool PossiblySensitive { get; }
        string SourceName { get; }
        string SourceUrl { get; }
        IStatusInfo QuotedStatus { get; }
        string SpoilerText { get; }
        string Text { get; }
        IUserInfo User { get; }
        long FavoriteCount { get; }
        long RetweetCount { get; }
        StatusVisibility Visibility { get; }
    }
}
