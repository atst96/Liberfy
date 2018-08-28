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
    internal class StatusInfo : NotificationObject
    {
        public SocialService Service { get; }

        public long Id { get; }

        public long[] Contributors { get; }

        public TwitterApi.Coordinates<TwitterApi.Point> Coordinates { get; }

        public DateTimeOffset CreatedAt { get; }

        private ITextEntityBuilder _textEntitiesBuilder;
        private IEnumerable<IEntity> _entities;
        public IEnumerable<IEntity> Entities
        {
            get
            {
                if (this._entities == null)
                    this._entities = this._textEntitiesBuilder.Build();

                return this._entities;
            }
        }

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

            this._textEntitiesBuilder = new TwitterTextTokenBuilder(this.Text ?? "", status.Entities);

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

        #region Mastodon process

        private StatusInfo(MastodonStatus status)
        {
            this.Service = SocialService.Twitter;

            if (status.Reblog != null)
                throw new ArgumentException();

            this.Id = status.Id;
            this.CreatedAt = status.CreatedAt;
            // this.FilterLevel = status.FilterLevel;
            this.InReplyToStatusId = status.InReplyToId ?? -1;
            this.InReplyToUserId = status.InReplyToAccountId ?? -1;

            // this.IsQuotedStatus = status.IsQuotedStatus && status.QuotedStatus != null;
            this.Language = status.Language;
            this.PossiblySensitive = status.Sensitive;

            this.SpoilerText = status.SpoilerText;
            this.Text = status.Content;

            this.User = DataStore.Mastodon.UserAddOrUpdate(status.Account);

            if (status.Application != null)
            {
                this.SourceName = status.Application.Name;
                this.SourceUrl = status.Application.Website;
            }

            this._textEntitiesBuilder = new MastodonTextEntityBuilder(this.Text ?? "");

            this.Attachments = status.MediaAttachments
                .Select(m => new Attachment(m))
                .ToArray() ?? new Attachment[0];

            this.Update(status);
        }

        public StatusInfo Update(MastodonStatus status)
        {
            if ((status.Reblog ?? status).Id != this.Id)
                throw new ArgumentException();

            this.FavoriteCount = status.FavouritesCount;
            this.RetweetCount = status.ReblogsCount;

            return this;
        }

        #endregion

        public StatusInfo Update(IStatus item)
        {
            switch (item)
            {
                case TwitterStatus twStatus:
                    return this.Update(twStatus);

                case MastodonStatus mdStatus:
                    return this.Update(mdStatus);

                default:
                    throw new NotImplementedException();
            }
        }

        public static StatusInfo Create(IStatus commonStatus)
        {
            switch (commonStatus)
            {
                case TwitterStatus twStatus:
                    return new StatusInfo(twStatus);

                case MastodonStatus mdStatus:
                    return new StatusInfo(mdStatus);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
