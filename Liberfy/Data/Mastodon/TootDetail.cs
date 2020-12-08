using System;
using System.Collections.Generic;
using Liberfy.Managers;
using Liberfy.Model;
using Liberfy.Services.Mastodon;
using SocialApis.Mastodon;

namespace Liberfy.Data.Mastodon
{
    /// <summary>
    /// Mastodonトゥート情報
    /// </summary>
    internal class TootDetail : NotificationObject, IStatusInfo<Status>
    {
        public ServiceType Service { get; } = ServiceType.Mastodon;

        public long Id { get; }
        public IReadOnlyList<long> Contributors { get; }
        public SocialApis.Twitter.Coordinates<SocialApis.Twitter.Point> Coordinates { get; }
        public DateTimeOffset CreatedAt { get; }
        public IReadOnlyList<Model.Attachment> Attachments { get; }
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

        public TootDetail(Status status, MastodonDataManager dataStore)
        {
            if (status.Reblog != null)
                throw new ArgumentException(nameof(status));

            this.Id = status.Id;
            this.CreatedAt = status.CreatedAt;
            this.InReplyToStatusId = status.InReplyToId ?? -1;
            this.InReplyToUserId = status.InReplyToAccountId ?? -1;
            this.Language = status.Language;
            this.PossiblySensitive = status.Sensitive;
            this.SpoilerText = status.SpoilerText;
            this.Text = status.Content ?? string.Empty;
            this.User = dataStore.RegisterAccount(status.Account);
            this.Attachments = GetAttachments(status.MediaAttachments);

            if (status.Application != null)
            {
                this.SourceName = status.Application.Name;
                this.SourceUrl = status.Application.Website;
            }

            this._textEntitiesBuilder = new MastodonTextEntityBuilder(this.Text, status.Emojis);

            this.Update(status);
        }

        public TootDetail Update(Status status)
        {
            this.FavoriteCount = status.FavouritesCount;
            this.RetweetCount = status.ReblogsCount;

            return this;
        }

        IStatusInfo<Status> IStatusInfo<Status>.Update(Status status)
        {
            return this.Update(status);
        }

        private static Model.Attachment[] GetAttachments(SocialApis.Mastodon.Attachment[] attachments)
        {
            int length = attachments?.Length ?? 0;
            var results = new Model.Attachment[length];

            for (int idx = 0; idx < length; ++idx)
            {
                results[idx] = new Model.Attachment(attachments[idx]);
            }

            return results;
        }
    }
}
