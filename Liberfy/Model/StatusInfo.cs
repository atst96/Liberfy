using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Liberfy.DataStore;

namespace Liberfy
{
    internal class StatusInfo : NotificationObject, IObjectInfo<Status>, IEquatable<StatusInfo>, IEquatable<Status>
    {
        public long Id { get; }

        //public Contributors[] Contributors { get; }

        public Coordinates<Point> Coordinates { get; }

        public DateTimeOffset CreatedAt { get; }

        public Entities Entities { get; }

        public ExtendedEntities ExtendedEntities { get; }

        public string FilterLevel { get; }

        public string InReplyToScreenName { get; }
        public long InReplyToStatusId { get; }
        public long InReplyToUserId { get; }

        public string Language { get; }

        public Places Place { get; }

        public bool PossiblySensitive { get; }
        // public bool PossiblySensitiveAppealable { get; }

        public string Source { get; }
        public string SourceName { get; }
        public string SourceUrl { get; }

        public bool IsQuotedStatus { get; }
        public StatusInfo QuotedStatus { get; }

        public string Text { get; }

        public UserInfo User { get; }

        // public bool WithheldCopyright { get; }

        public string[] WithheldInCountries { get; }

        // public Dictionary<string, object> Scopes { get; }
        public string WithheldScope { get; }

        public long FavoriteCount { get; private set; }
        public long RetweetCount { get; private set; }

        long IObjectInfo<Status>.Id => this.Id;

        public StatusInfo(Status status)
        {
            if (status.RetweetedStatus != null)
                throw new ArgumentException();

            this.Id = status.Id;

            // this.Contributors     = status.Contributors;
            this.Coordinates      = status.Coordinates;
            this.CreatedAt        = status.CreatedAt;
            this.Entities         = status.Entities;
            this.ExtendedEntities = status.ExtendedEntities;
            this.FilterLevel      = status.FilterLevel;

            this.InReplyToScreenName = status.InReplyToScreenName;
            this.InReplyToStatusId   = status.InReplyToStatusId ?? -1;
            this.InReplyToUserId     = status.InReplyToUserId ?? -1;

            this.IsQuotedStatus = status.IsQuotedStatus && status.QuotedStatus != null;

            this.Language = status.Language;

            this.Place = status.Place;

            this.PossiblySensitive = status.PossiblySensitive;
            // this.PossiblySensitiveAppealable = status.PossiblySensitiveAppealable;

            if (this.IsQuotedStatus)
                this.QuotedStatus = StatusAddOrUpdate(status.QuotedStatus);

            // this.Scopes = status.Scopes;

            this.Text = status.Text;

            this.User = UserAddOrUpdate(status.User);

            // this.WithheldCopyright = status.WithheldCopyright ?? false;
            this.WithheldInCountries = status.WithheldInCountries;
            this.WithheldScope = status.WithheldScope;

            this.Source = status.Source;
            var regex = Regexes.ATagSource.Match(status.Source);
            if (regex.Success)
            {
                this.SourceName = regex.Groups["name"].Value;
                this.SourceUrl = regex.Groups["url"].Value;
            }

            this.Update(status);
        }

        public StatusInfo Update(Status item)
        {
            if ((item.RetweetedStatus ?? item).Id != Id)
                throw new ArgumentException();

            this.FavoriteCount = item.FavoriteCount ?? this.FavoriteCount;
            this.RetweetCount = item.RetweetCount ?? this.RetweetCount;

            this.RaisePropertyChanged(nameof(this.FavoriteCount));
            this.RaisePropertyChanged(nameof(this.RetweetCount));

            return this;
        }

        void IObjectInfo<Status>.Update(Status item) => this.Update(item);

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
