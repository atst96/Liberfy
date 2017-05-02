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
		public ItemType Type { get; }

		public Account Account { get; }

		public long Id { get; }

		public DateTimeOffset CreatedAt { get; }

		public bool IsRetweet { get; }

		public bool IsReply { get; }

		public string InReplyToScreenName { get; }

		public StatusInfo Status { get; }

		public UserInfo User { get; }

		public UserInfo RetweetUser { get; }

		public bool IsMe { get; }

		public Reaction Reaction { get; }

		public MediaEntityInfo[] MediaEntities { get; }

		public bool HasMediaEntities { get; }

		public StatusItem(Status status, Account account)
		{
			Id = status.Id;

			Reaction = account.GetStatusReaction(status.GetSourceId());

			Reaction.IsRetweeted = status.IsRetweeted ?? false;
			Reaction.IsFavorited = status.IsFavorited ?? false;

			if(IsRetweet = status.RetweetedStatus != null)
			{
				Type = ItemType.Retweet;
				Status = StatusAddOrUpdate(status.RetweetedStatus);
				RetweetUser = UserAddOrUpdate(status.User);

				if(status.User.Id == account.Id)
				{
					Reaction.IsRetweeted = true;
				}
			}
			else
			{
				Type = ItemType.Status;
				Status = StatusAddOrUpdate(status);

				if(IsReply = status.InReplyToStatusId.HasValue)
				{
					InReplyToScreenName = status.InReplyToScreenName;
				}
			}

			User = Status.User;
			CreatedAt = Status.CreatedAt;
			MediaEntities = Status.ExtendedEntities?
				.Media.Select(mediaEntity => new MediaEntityInfo(account, this, mediaEntity))
				.ToArray();

			IsMe = Status.User.Id == account.Id;
		}

		public void RaiseCreatedAtProeprtyChanged()
		{
			RaisePropertyChanged(nameof(CreatedAt));
		}
	}
}
