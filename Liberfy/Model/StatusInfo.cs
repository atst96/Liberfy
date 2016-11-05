using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class StatusInfo : NotificationObject, IObjectInfo<Status>, IEquatable<StatusInfo>, IEquatable<Status>
	{
		public long Id { get; private set; }

		public Contributors[] Contributors { get; private set; }

		public Coordinates Coordinates { get; private set; }

		public DateTimeOffset CreatedAt { get; private set; }

		public int[] DisplayTextRange { get; private set; }

		public Entities Entities { get; private set; }

		public Entities ExtendedEntities { get; private set; }

		public CompatExtendedTweet ExtendedTweet { get; private set; }

		public int FavoriteCount { get; private set; }

		public FilterLevel FilterLevel { get; private set; }

		public string FullText { get; private set; }

		public string InReplyToScreenName { get; private set; }

		public long InReplyToStatusId { get; private set; }

		public long InReplyToUserId { get; private set; }

		public bool IsQuotedStatus { get; private set; }

		public string Language { get; private set; }

		public Place Place { get; private set; }

		public bool PossiblySensitive { get; private set; }

		public bool PossiblySensitiveAppealable { get; private set; }

		public long QuotedStatusId { get; private set; }

		public Status QuotedStatus { get; private set; }

		public Dictionary<string, object> Scopes { get; private set; }

		public int RetweetCount { get; private set; }

		public Status RetweetedStatus { get; private set; }

		public string Source { get; private set; }

		public string Text { get; private set; }

		public User User { get; private set; }

		public bool WithheldCopyright { get; private set; }

		public string WithheldInCountries { get; private set; }

		public string WithheldScope { get; private set; }


		public StatusInfo(Status item)
		{
			Id = item.Id;
			Contributors = item.Contributors;
			Coordinates = item.Coordinates;
			CreatedAt = item.CreatedAt;
			DisplayTextRange = item.DisplayTextRange;
			Entities = item.Entities;
			ExtendedEntities = item.ExtendedEntities;
			ExtendedTweet = item.ExtendedTweet;
			FilterLevel = item.FilterLevel ?? FilterLevel.None;
			FullText = item.FullText;
			InReplyToScreenName = item.InReplyToScreenName;
			InReplyToStatusId = item.InReplyToStatusId ?? -1;
			InReplyToUserId = item.InReplyToUserId ?? -1;
			IsQuotedStatus = item.IsQuotedStatus ?? false;
			Language = item.Language;
			Place = item.Place;
			PossiblySensitive = item.PossiblySensitive ?? PossiblySensitive;
			PossiblySensitiveAppealable = item.PossiblySensitiveAppealable ?? PossiblySensitiveAppealable;
			QuotedStatusId = item.QuotedStatusId ?? -1;
			QuotedStatus = item.QuotedStatus;
			Scopes = item.Scopes;
			Source = item.Source;
			Text = item.Text;
			User = item.User;
			WithheldCopyright = item.WithheldCopyright ?? WithheldCopyright;
			WithheldInCountries = item.WithheldInCountries;
			WithheldScope = item.WithheldScope;
			RetweetedStatus = item.RetweetedStatus;
		}

		public void Update(Status item)
		{
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
