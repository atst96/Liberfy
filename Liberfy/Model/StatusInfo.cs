using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Liberfy.DataStore;

namespace Liberfy
{
	class StatusInfo :
		NotificationObject,
		IObjectInfo<Status>,
		IEquatable<StatusInfo>,
		IEquatable<Status>
	{
		public long Id { get; }

		public Contributors[] Contributors { get; }

		public Coordinates Coordinates { get; }

		public DateTimeOffset CreatedAt { get; }

		public int[] DisplayTextRange { get; }

		public Entities Entities { get; }

		public Entities ExtendedEntities { get; }

		public CompatExtendedTweet ExtendedTweet { get; }

		public FilterLevel FilterLevel { get; }

		public string FullText { get; }

		public string InReplyToScreenName { get; }

		public long InReplyToStatusId { get; }

		public long InReplyToUserId { get; }

		public bool IsQuotedStatus { get; }

		public string Language { get; }

		public Place Place { get; }

		public bool PossiblySensitive { get; }

		public bool PossiblySensitiveAppealable { get; }

		public long QuotedStatusId { get; }

		public StatusInfo QuotedStatus { get; }

		public Dictionary<string, object> Scopes { get; }

		public string Source { get; }

		public string Text { get; }

		public UserInfo User { get; }

		public bool WithheldCopyright { get; }

		public string WithheldInCountries { get; }

		public string WithheldScope { get; }

		private int _favoriteCount;
		public int FavoriteCount
		{
			get { return _favoriteCount; }
			set { SetProperty(ref _favoriteCount, value); }
		}

		private int _retweetCount;
		public int RetweetCount
		{
			get { return _retweetCount; }
			set { SetProperty(ref _retweetCount, value); }
		}


		public StatusInfo(Status status)
		{
			if(status.RetweetedStatus != null)
			{
				throw new ArgumentException("StatusInfo はリツート情報を保持しません");
			}

			Id = status.Id;
			Contributors = status.Contributors;
			Coordinates = status.Coordinates;
			CreatedAt = status.CreatedAt;
			DisplayTextRange = status.DisplayTextRange;
			Entities = status.Entities;
			ExtendedEntities = status.ExtendedEntities;
			ExtendedTweet = status.ExtendedTweet;
			FilterLevel = status.FilterLevel ?? FilterLevel.None;
			FullText = status.FullText;
			InReplyToScreenName = status.InReplyToScreenName;
			InReplyToStatusId = status.InReplyToStatusId ?? -1;
			InReplyToUserId = status.InReplyToUserId ?? -1;
			IsQuotedStatus = status.IsQuotedStatus ?? false;
			Language = status.Language;
			Place = status.Place;
			PossiblySensitive = status.PossiblySensitive ?? PossiblySensitive;
			PossiblySensitiveAppealable = status.PossiblySensitiveAppealable ?? PossiblySensitiveAppealable;
			QuotedStatusId = status.QuotedStatusId ?? -1;
			QuotedStatus = StatusAddOrUpdate(status.QuotedStatus);
			Scopes = status.Scopes;
			Source = status.Source;
			Text = status.Text;
			User = UserAddOrUpdate(status.User);
			WithheldCopyright = status.WithheldCopyright ?? WithheldCopyright;
			WithheldInCountries = status.WithheldInCountries;
			WithheldScope = status.WithheldScope;
		}

		public void Update(Status item)
		{
			if((item.RetweetedStatus??item).Id != Id)
			{
				throw new ArgumentException();
			}

			FavoriteCount = item.FavoriteCount ?? FavoriteCount;
			RetweetCount = item.RetweetCount ?? RetweetCount;
		}

		public bool Equals(StatusInfo user)
		{
			return Equals(Id, user?.Id);
		}

		public bool Equals(Status user)
		{
			return Equals(Id, user?.Id);
		}

		public override bool Equals(object obj)
		{
			return obj is StatusInfo && Equals((StatusInfo)obj)
				|| obj is Status && Equals((Status)obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
