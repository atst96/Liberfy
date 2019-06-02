using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Model;
using Liberfy.Services.Twitter;
using SocialApis.Twitter;

namespace Liberfy
{
    internal class TwitterStatusInfo : NotificationObject, IStatusInfo<Status>
    {
        public ServiceType Service { get; } = ServiceType.Twitter;

        public long Id { get; }
        public long[] Contributors { get; }
        public SocialApis.Twitter.Coordinates<SocialApis.Twitter.Point> Coordinates { get; set; }
        public DateTimeOffset CreatedAt { get; }
        public Attachment[] Attachments { get; }
        public string FilterLevel { get; }
        public long InReplyToStatusId { get; }
        public long InReplyToUserId { get; }
        public string Language { get; }
        public SocialApis.Twitter.Places Place { get; }
        public bool PossiblySensitive { get; }
        public string SourceName { get; }
        public string SourceUrl { get; }
        public IStatusInfo QuotedStatus { get; }
        public string SpoilerText { get; }
        public string Text { get; }
        public IUserInfo User { get; }
        public StatusVisibility Visibility { get; }

        private ITextEntityBuilder _textEntitiesBuilder;
        private IEnumerable<IEntity> _entities;
        public IEnumerable<IEntity> Entities
        {
            get
            {
                if (this._entities == null)
                {
                    this._entities = this._textEntitiesBuilder.Build();
                    this._textEntitiesBuilder = null;
                }

                return this._entities;
            }
        }

        private long _favoriteCount;
        public long FavoriteCount
        {
            get => this._favoriteCount;
            private set => this.SetProperty(ref this._favoriteCount, value);
        }

        private long _retweetCount;
        public long RetweetCount
        {
            get => this._retweetCount;
            private set => this.SetProperty(ref this._retweetCount, value);
        }

        public TwitterStatusInfo(Status status, TwitterDataStore dataStore)
        {
            if (status.RetweetedStatus != null)
                throw new ArgumentException();

            this.Id = status.Id;
            this.CreatedAt = status.CreatedAt;
            this.FilterLevel = status.FilterLevel;
            this.InReplyToStatusId = status.InReplyToStatusId ?? -1;
            this.InReplyToUserId = status.InReplyToUserId ?? -1;
            this.Language = status.Language;
            this.PossiblySensitive = status.PossiblySensitive;
            this.Text = status.FullText ?? status.Text ?? string.Empty;
            this.User = dataStore.RegisterAccount(status.User);
            this.Attachments = GetAttachments(status.ExtendedEntities?.Media);

            if (status.QuotedStatus != null)
            {
                this.QuotedStatus = dataStore.RegisterStatus(status.QuotedStatus);
            }

            if (status.Place != null)
            {

            }

            if (status.Contributors != null)
            {

            }

            if (status.Coordinates != null)
            {

            }

            (this.SourceUrl, this.SourceName) = ExpandClientInfo(status);

            this._textEntitiesBuilder = new TwitterTextTokenBuilder(this.Text, status.Entities);

            this.Update(status);
        }

        public TwitterStatusInfo Update(Status status)
        {
            if (this.Id != (status.RetweetedStatus ?? status).Id)
                throw new ArgumentException(nameof(status.Id));

            this.FavoriteCount = status.FavoriteCount ?? this.FavoriteCount;
            this.RetweetCount = status.RetweetCount ?? this.RetweetCount;

            return this;
        }

        IStatusInfo<Status> IStatusInfo<Status>.Update(Status status)
        {
            return this.Update(status);
        }

        private static Attachment[] GetAttachments(SocialApis.Twitter.MediaEntity[] entities)
        {
            var results = new Attachment[entities?.Length ?? 0];

            for (int idx = 0; idx < results.Length; ++idx)
            {
                results[idx] = new Attachment(entities[idx]);
            }

            return results;
        }

        private static (string url, string sourceName) ExpandClientInfo(Status status)
        {
            var match = Regexes.TwitterSourceHtml.Match(status.Source);

            return match?.Success ?? false
                ? (string.Empty, status.Source)
                : (match.Groups["url"].Value, match.Groups["name"].Value);
        }
    }
}
