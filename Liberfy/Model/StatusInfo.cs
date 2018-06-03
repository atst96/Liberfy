using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterStatus = SocialApis.Twitter.Status;
using MastodonStatus = SocialApis.Mastodon.Status;
using SocialApis;
using SocialApis.Common;

namespace Liberfy
{
    internal class StatusInfo : NotificationObject, IEquatable<StatusInfo>, IEquatable<Status>
    {
        public long Id { get; }

        public long[] Contributors { get; }

        public Coordinates<Point> Coordinates { get; }

        public DateTimeOffset CreatedAt { get; }

        public SocialApis.Common.EntityBase[] Entities { get; }

        public SocialApis.Common.Attachment[] Attachments { get; }

        public string FilterLevel { get; }

        public long InReplyToStatusId { get; }
        public long InReplyToUserId { get; }

        public string Language { get; }

        public Places Place { get; }

        public bool PossiblySensitive { get; }

        public string SourceName { get; }
        public string SourceUrl { get; }

        public bool IsQuotedStatus { get; }
        public StatusInfo QuotedStatus { get; }

        public string SpoilerText { get; }

        public string Text { get; }

        public UserInfo User { get; }

        public long FavoriteCount { get; private set; }
        public long RetweetCount { get; private set; }

        public SocialApis.Mastodon.StatusVisibility Visibility { get; }

        public StatusInfo(ICommonStatus status)
        {
            if (status.RetweetedStatus != null)
                throw new ArgumentException();

            this.Id = status.Id;

            this.CreatedAt = status.CreatedAt;
            this.FilterLevel = status.FilterLevel;

            this.InReplyToStatusId = status.InReplyToStatusId ?? -1;
            this.InReplyToUserId = status.InReplyToUserId ?? -1;

            this.IsQuotedStatus = status.IsQuotedStatus && status.QuotedStatus != null;

            this.Language = status.Language;


            this.PossiblySensitive = status.IsSensitive;

            if (this.IsQuotedStatus)
                this.QuotedStatus = DataStore.Twitter.StatusAddOrUpdate(status.QuotedStatus);

            this.Text = status.Text;

            this.User = DataStore.Twitter.UserAddOrUpdate(status.User);

            this.SourceName = status.SourceName;
            this.SourceUrl = status.SourceUrl;

            this.Entities = status.Entities;

            this.Attachments = status.Attachments;

            this.Update(status);
        }

        public StatusInfo Update(ICommonStatus item)
        {
            if ((item.RetweetedStatus ?? item).Id != Id)
                throw new ArgumentException();

            this.FavoriteCount = item.FavoriteCount ?? this.FavoriteCount;
            this.RetweetCount = item.RetweetCount ?? this.RetweetCount;

            this.RaisePropertyChanged(nameof(this.FavoriteCount));
            this.RaisePropertyChanged(nameof(this.RetweetCount));

            return this;
        }

        //void IObjectInfo<Status>.Update(Status item) => this.Update(item);

        public override bool Equals(object obj)
        {
            return (obj is StatusInfo statusInfo && this.Equals(statusInfo))
                || (obj is Status status && this.Equals(status));
        }

        public bool Equals(StatusInfo other) => object.Equals(Id, other?.Id);

        public bool Equals(Status other) => object.Equals(Id, other?.Id);

        public override int GetHashCode() => Id.GetHashCode();
    }
}
