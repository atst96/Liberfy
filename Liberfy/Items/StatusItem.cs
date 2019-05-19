﻿using SocialApis.Twitter;
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
        public StatusInfo Status { get; }
        public UserInfo User { get; }
        public UserInfo RetweetUser { get; }
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

            var reaction = account.GetActivity(status.GetSourceId());
            reaction.SetAll(status.IsFavorited ?? false, status.IsRetweeted ?? false);

            StatusInfo statusInfo;

            this.IsRetweet = status.RetweetedStatus != null;
            if (this.IsRetweet)
            {
                this.Type = ItemType.Retweet;

                statusInfo = DataStore.Twitter.RegisterStatus(status.RetweetedStatus);
                this.RetweetUser = DataStore.Twitter.RegisterAccount(status.User);

                if (status.User.Id == account.Id)
                    reaction.IsRetweeted = true;
            }
            else
            {
                this.Type = ItemType.Status;
                statusInfo = DataStore.Twitter.RegisterStatus(status);

                this.IsReply = status.InReplyToStatusId.HasValue;
                if (this.IsReply)
                    this.InReplyToScreenName = status.InReplyToScreenName;
            }


            this.Reaction = reaction;

            this.Status = statusInfo;
            this.User = statusInfo.User;
            this.CreatedAt = statusInfo.CreatedAt;
            this.MediaEntities = statusInfo.Attachments.Select(mediaEntity => new MediaEntityInfo(account, this, mediaEntity));
            this.HasMediaEntities = this.MediaEntities?.Any() ?? false;
            this.IsCurrentAccount = statusInfo.User.Id == account.Id;

            this.User.PropertyChanged += this.OnUserPropertyChanged;

            this.Commands = new StatusCommandGroup(this);
        }

        public StatusItem(MastodonStatus status, MastodonAccount account)
        {
            this.Id = status.Id;
            this.Account = account;

            var reaction = account.GetActivity((status.Reblog??status).Id);
            reaction.SetAll(status.Favourited ?? false, status.Reblogged ?? false);

            StatusInfo statusInfo;

            var dataStore = DataStore.Mastodon[account.Tokens.HostUrl];

            this.IsRetweet = status.Reblog != null;
            if (this.IsRetweet)
            {
                this.Type = ItemType.Retweet;

                statusInfo = dataStore.RegisterStatus(status.Reblog);
                this.RetweetUser = dataStore.RegisterAccount(status.Account);

                if (status.Account.Id == account.Id)
                    reaction.IsRetweeted = true;
            }
            else
            {
                this.Type = ItemType.Status;
                statusInfo = dataStore.RegisterStatus(status);

                this.IsReply = status.InReplyToId.HasValue;
                if (this.IsReply)
                    this.InReplyToScreenName = status.Account.Acct;
            }

            this.Reaction = reaction;

            this.Status = statusInfo;
            this.User = statusInfo.User;
            this.CreatedAt = statusInfo.CreatedAt;
            this.MediaEntities = statusInfo.Attachments.Select(mediaEntity => new MediaEntityInfo(account, this, mediaEntity));
            this.HasMediaEntities = this.MediaEntities?.Any() ?? false;
            this.IsCurrentAccount = statusInfo.User.Id == account.Id;

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

        // TODO: 何のために作ったメソッドかを忘れた
        // public void RaiseCreatedAtProeprtyChanged() => this.RaisePropertyChanged(nameof(CreatedAt));
    }
}