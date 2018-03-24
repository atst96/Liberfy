using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Liberfy.DataStore;

namespace Liberfy
{
    internal class StatusItem : NotificationObject, IItem
    {
        public Account Account { get; }

        public long Id { get; }
        public DateTimeOffset CreatedAt { get; }
        public bool IsRetweet { get; }
        public bool IsReply { get; }
        public string InReplyToScreenName { get; }
        public StatusInfo Status { get; }
        public UserInfo User { get; }
        public UserInfo RetweetUser { get; }
        public bool IsCurrentAccount { get; }
        public StatusReaction Reaction { get; }
        public IEnumerable<MediaEntityInfo> MediaEntities { get; }
        public bool HasMediaEntities { get; }

        public ItemType Type { get; }

        public StatusItem(Status status, Account account)
        {
            this.Id = status.Id;

            var reaction = account.GetStatusReaction(status.GetSourceId());
            reaction.SetAll(status.IsFavorited ?? false, status.IsRetweeted ?? false);

            StatusInfo statusInfo;

            this.IsRetweet = status.RetweetedStatus != null;
            if (this.IsRetweet)
            {
                this.Type = ItemType.Retweet;

                statusInfo = StatusAddOrUpdate(status.RetweetedStatus);
                this.RetweetUser = UserAddOrUpdate(status.User);

                if (status.User.Id == account.Id)
                    reaction.IsRetweeted = true;
            }
            else
            {
                this.Type = ItemType.Status;
                statusInfo = StatusAddOrUpdate(status);

                this.IsReply = status.InReplyToStatusId.HasValue;
                if (this.IsReply)
                    this.InReplyToScreenName = status.InReplyToScreenName;
            }


            this.Reaction = reaction;

            this.Status           = statusInfo;
            this.User             = statusInfo.User;
            this.CreatedAt        = statusInfo.CreatedAt;
            this.MediaEntities    = statusInfo.ExtendedEntities?.Media?.Select(mediaEntity => new MediaEntityInfo(account, this, mediaEntity));
            this.HasMediaEntities = MediaEntities?.Any() ?? false;
            this.IsCurrentAccount = statusInfo.User.Id == account.Id;
        }

        // TODO: 何のために作ったメソッドかを忘れた
        // public void RaiseCreatedAtProeprtyChanged() => this.RaisePropertyChanged(nameof(CreatedAt));
    }
}
