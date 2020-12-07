using System;
using System.Collections.Generic;
using Liberfy.Factories;
using Liberfy.Model;
using Liberfy.Services.Twitter;
using SocialApis.Twitter;

namespace Liberfy.Data.Twitter
{
    /// <summary>
    /// ツイート情報
    /// </summary>
    internal class TweetDetail : NotificationObject, IStatusInfo<Status>
    {
        public ServiceType Service { get; } = ServiceType.Twitter;

        public long Id { get; }
        public IReadOnlyList<long> Contributors { get; }
        public SocialApis.Twitter.Coordinates<SocialApis.Twitter.Point> Coordinates { get; }
        public DateTimeOffset CreatedAt { get; }
        public IReadOnlyList<Attachment> Attachments { get; }
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
        private IReadOnlyList<IEntity> _entities;
        public IReadOnlyList<IEntity> Entities
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

        public TweetDetail(Status status, TwitterDataFactory dataStore)
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

        public TweetDetail Update(Status status)
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
            int length = entities?.Length ?? 0;
            var attachments = new Attachment[length];

            for (int idx = 0; idx < length; ++idx)
            {
                attachments[idx] = new Attachment(entities[idx]);
            }

            return attachments;
        }

        private static (string url, string sourceName) ExpandClientInfo(Status status)
        {
            var match = Regexes.TwitterSourceHtml.Match(status.Source);

            return match.Success
                ? (match.Groups["url"].Value, match.Groups["name"].Value)
                : (string.Empty, match.Value);
        }
    }
}
