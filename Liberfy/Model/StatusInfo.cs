using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterStatus = SocialApis.Twitter.Status;
using MastodonStatus = SocialApis.Mastodon.Status;
using System.ComponentModel;
using Liberfy.Model;

using TwitterApi = SocialApis.Twitter;
using SocialApis;
using SocialApis.Common;

namespace Liberfy
{
    internal class StatusInfo : NotificationObject, IEquatable<StatusInfo>, IEquatable<TwitterStatus>
    {
        public SocialService Service { get; }

        public long Id { get; }

        public long[] Contributors { get; }

        public TwitterApi.Coordinates<TwitterApi.Point> Coordinates { get; }

        public DateTimeOffset CreatedAt { get; }

        public EntityBase[] Entities { get; }

        public Attachment[] Attachments { get; }

        public string FilterLevel { get; }

        public long InReplyToStatusId { get; }
        public long InReplyToUserId { get; }

        public string Language { get; }

        public TwitterApi.Places Place { get; }

        public bool PossiblySensitive { get; }

        public string SourceName { get; }
        public string SourceUrl { get; }

        public bool IsQuotedStatus { get; }
        public StatusInfo QuotedStatus { get; }

        public string SpoilerText { get; }

        public string Text { get; }

        public UserInfo User { get; }

        private long _favoriteCount;
        private static readonly PropertyChangedEventArgs _favoriteCountPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(StatusInfo.FavoriteCount));
        public long FavoriteCount
        {
            get => this._favoriteCount;
            private set => this.SetProperty(ref this._favoriteCount, value, _favoriteCountPropertyChangedEventArgs);
        }

        private long _retweetCount;
        private static readonly PropertyChangedEventArgs _retweetCountPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(StatusInfo.RetweetCount));
        public long RetweetCount
        {
            get => this._retweetCount;
            private set => this.SetProperty(ref this._retweetCount, value, _retweetCountPropertyChangedEventArgs);
        }

        public SocialApis.Mastodon.StatusVisibility Visibility { get; }

        #region Begin: Twitter process

        private StatusInfo(TwitterStatus status)
        {
            this.Service = SocialService.Twitter;

            if (status.RetweetedStatus != null)
                throw new ArgumentException();

            this.Id = status.Id;
            this.CreatedAt = status.CreatedAt;
            this.FilterLevel = status.FilterLevel;
            this.InReplyToStatusId = status.InReplyToStatusId ?? -1;
            this.InReplyToUserId = status.InReplyToUserId ?? -1;

            this.IsQuotedStatus = status.IsQuotedStatus && status.QuotedStatus != null;
            this.Language = status.Language;
            this.PossiblySensitive = status.PossiblySensitive;

            if (this.IsQuotedStatus)
                this.QuotedStatus = DataStore.Twitter.StatusAddOrUpdate(status.QuotedStatus);

            this.Text = status.FullText ?? status.Text;

            this.User = DataStore.Twitter.UserAddOrUpdate(status.User);

            (this.SourceUrl, this.SourceName) = status.ParseSource();

            this.Entities = status.Entities
                .ToEntityList()
                .ToCommonEntities(this.Text) ?? new EntityBase[0];

            this.Attachments = status.ExtendedEntities?.Media
                .Select(m => new Attachment(m))
                .ToArray() ?? new Attachment[0];

            this.Update(status);
        }

        public StatusInfo Update(TwitterStatus status)
        {
            if ((status.RetweetedStatus ?? status).Id != this.Id)
                throw new ArgumentException();

            this.FavoriteCount = status.FavoriteCount ?? this.FavoriteCount;
            this.RetweetCount = status.RetweetCount ?? this.RetweetCount;

            return this;
        }

        #endregion End: Twitter process

        public StatusInfo Update(IStatus item)
        {
            if (item is TwitterStatus twitterStatus)
                return this.Update(twitterStatus);
            else
                throw new NotImplementedException();
        }

        public static StatusInfo Create(IStatus commonStatus)
        {
            if (commonStatus is TwitterStatus twitterStatus)
                return new StatusInfo(twitterStatus);
            else
                throw new NotImplementedException();
        }

        //void IObjectInfo<Status>.Update(Status item) => this.Update(item);

        public override bool Equals(object obj)
        {
            return (obj is StatusInfo statusInfo && this.Equals(statusInfo))
                || (obj is TwitterStatus status && this.Equals(status));
        }

        public bool Equals(StatusInfo other) => object.Equals(Id, other?.Id);

        public bool Equals(TwitterStatus other) => object.Equals(Id, other?.Id);

        public override int GetHashCode() => Id.GetHashCode();
    }
}
