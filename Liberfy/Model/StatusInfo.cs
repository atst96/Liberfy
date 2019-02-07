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
    internal class StatusInfo : NotificationObject
    {
        public ServiceType Service { get; set; }

        public long Id { get; set; }

        public long[] Contributors { get; set; }

        public TwitterApi.Coordinates<TwitterApi.Point> Coordinates { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

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

        public Attachment[] Attachments { get; set; }

        public string FilterLevel { get; set; }

        public long InReplyToStatusId { get; set; }
        public long InReplyToUserId { get; set; }

        public string Language { get; set; }

        public TwitterApi.Places Place { get; set; }

        public bool PossiblySensitive { get; set; }

        public string SourceName { get; set; }
        public string SourceUrl { get; set; }

        public bool IsQuotedStatus { get; set; }
        public StatusInfo QuotedStatus { get; set; }

        public string SpoilerText { get; set; }

        public string Text { get; set; }

        public UserInfo User { get; set; }

        private long _favoriteCount;
        private static readonly PropertyChangedEventArgs _favoriteCountPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(StatusInfo.FavoriteCount));
        public long FavoriteCount
        {
            get => this._favoriteCount;
            set => this.SetProperty(ref this._favoriteCount, value, _favoriteCountPropertyChangedEventArgs);
        }

        private long _retweetCount;
        private static readonly PropertyChangedEventArgs _retweetCountPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(StatusInfo.RetweetCount));
        public long RetweetCount
        {
            get => this._retweetCount;
            set => this.SetProperty(ref this._retweetCount, value, _retweetCountPropertyChangedEventArgs);
        }

        public SocialApis.Mastodon.StatusVisibility Visibility { get; }

        public StatusInfo(ServiceType service)
        {
            this.Service = service;
        }

        public void SetTextEntityBuilder(ITextEntityBuilder builder)
        {
            this._textEntitiesBuilder = builder;
        }
    }
}
