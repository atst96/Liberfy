using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterStatus = SocialApis.Twitter.Status;
using MastodonStatus = SocialApis.Mastodon.Status;
using Liberfy.ViewModels;
using Liberfy.Commands;

namespace Liberfy
{
    internal class StatusItem : NotificationObject, IItem
    {
        public IAccount Account { get; }

        public long Id { get; }
        public DateTimeOffset CreatedAt { get; }
        public bool IsRetweet { get; }
        public bool IsReply { get; }
        public string InReplyToScreenName { get; }
        public IStatusInfo Status { get; }
        public IUserInfo User { get; }
        public IUserInfo RetweetUser { get; }
        public bool IsCurrentAccount { get; }
        public StatusActivity Reaction { get; }
        public IEnumerable<MediaEntityInfo> MediaEntities { get; }
        public bool HasMediaEntities { get; }

        public StatusCommandGroup Commands { get; }

        public bool CanRetweet
        {
            // 公開ユーザまたは自身のアカウントの場合
            get => !this.User.IsProtected || this.Account.Equals(this.User);
        }

        public ItemType Type { get; }

        public bool IsAnimatable { get; }

        public StatusItem(TwitterStatus status, TwitterAccount account)
        {
            this.Id = status.Id;
            this.Account = account;

            var dataStore = account.DataStore;

            IStatusInfo info;

            if (status.RetweetedStatus != null)
            {
                this.Type = ItemType.Retweet;
                this.IsRetweet = true;

                info = dataStore.RegisterStatus(status.RetweetedStatus);
                this.RetweetUser = dataStore.RegisterAccount(status.User);
            }
            else
            {
                this.Type = ItemType.Status;

                info = dataStore.RegisterStatus(status);

                if (status.InReplyToStatusId.HasValue)
                {
                    this.IsReply = true;
                    this.InReplyToScreenName = status.InReplyToScreenName;
                }
            }


            this.Reaction = account.GetStatusActivity(info.Id);
            this.Reaction.Set(status.IsFavorited, status.IsRetweeted);

            this.Status = info;
            this.User = info.User;
            this.CreatedAt = info.CreatedAt;
            this.MediaEntities = info.Attachments.Select(mediaEntity => new MediaEntityInfo(account, this, mediaEntity));
            this.HasMediaEntities = this.MediaEntities?.Any() ?? false;
            this.IsCurrentAccount = info.User.Id == account.Id;

            this.User.PropertyChanged += this.OnUserPropertyChanged;

            this.Commands = new StatusCommandGroup(this);
        }

        public StatusItem(MastodonStatus status, MastodonAccount account)
        {
            this.Id = status.Id;
            this.Account = account;

            IStatusInfo info;

            var dataStore = account.DataStore;

            if (status.Reblog != null)
            {
                this.Type = ItemType.Retweet;
                this.IsRetweet = true;

                info = dataStore.RegisterStatus(status.Reblog);
                this.RetweetUser = dataStore.RegisterAccount(status.Account);
            }
            else
            {
                this.Type = ItemType.Status;

                info = dataStore.RegisterStatus(status);

                if (status.InReplyToId.HasValue)
                {
                    this.IsReply = true;
                    //this.InReplyToScreenName = status.InReplyToId;
                }
            }

            this.Reaction = account.GetStatusActivity(info.Id);
            this.Reaction.Set(status.Favourited, status.Reblogged);

            this.Status = info;
            this.User = info.User;
            this.CreatedAt = info.CreatedAt;
            this.MediaEntities = info.Attachments.Select(mediaEntity => new MediaEntityInfo(account, this, mediaEntity));
            this.HasMediaEntities = this.MediaEntities?.Any() ?? false;
            this.IsCurrentAccount = info.User.Id == account.Id;

            this.User.PropertyChanged += this.OnUserPropertyChanged;

            this.Commands = new StatusCommandGroup(this);
        }

        private void OnUserPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.User.IsProtected))
            {
                this.RaisePropertyChanged(nameof(this.CanRetweet));
            }
        }

        // public void RaiseCreatedAtProeprtyChanged() => this.RaisePropertyChanged(nameof(CreatedAt));
    }
}
