using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Liberfy.DataStore;

namespace Liberfy
{
	internal class StatusInfo : NotificationObject, IObjectInfo<Status>, IEquatable<StatusInfo>, IEquatable<Status>
	{
		private readonly Status _status;

		private readonly long _id;
		public long Id => _id;

		public Contributors[] Contributors => _status.Contributors;

		public Coordinates Coordinates => _status.Coordinates;

		public DateTimeOffset CreatedAt => _status.CreatedAt;

		public int[] DisplayTextRange => _status.DisplayTextRange;

		public Entities Entities => _status.Entities;

		public Entities ExtendedEntities => _status.ExtendedEntities;

		public CompatExtendedTweet ExtendedTweet => _status.ExtendedTweet;

		public FilterLevel FilterLevel => _status.FilterLevel ?? FilterLevel.None;

		public string FullText => _status.FullText;

		public string InReplyToScreenName => _status.InReplyToScreenName;

		public long InReplyToStatusId => _status.InReplyToStatusId ?? -1;

		public long InReplyToUserId => _status.InReplyToUserId ?? -1;

		public bool IsQuotedStatus => _status.IsQuotedStatus ?? false;

		public string Language => _status.Language;

		public Place Place => _status.Place;

		public bool PossiblySensitive => _status.PossiblySensitive ?? false;

		public bool PossiblySensitiveAppealable => _status.PossiblySensitiveAppealable ?? false;

		public long QuotedStatusId => _status.QuotedStatusId ?? -1;

		private StatusInfo _quotedStatus;
		public StatusInfo QuotedStatus
		{
			get => IsQuotedStatus ? (_quotedStatus ?? (_quotedStatus = StatusAddOrUpdate(_status.QuotedStatus))) : null;
		}

		public Dictionary<string, object> Scopes => _status.Scopes;

		public string Source => _status.Source;

		public string Text => _status.Text;

		private UserInfo _user;
		public UserInfo User
		{
			get => _user ?? (_user = UserAddOrUpdate(_status.User));
		}

		public bool WithheldCopyright => _status.WithheldCopyright ?? false;

		public string WithheldInCountries => _status.WithheldInCountries;

		public string WithheldScope => _status.WithheldScope;

		private int _favoriteCount;
		public int FavoriteCount
		{
			get => _favoriteCount;
			private set => SetProperty(ref _favoriteCount, value);
		}

		private int _retweetCount;
		public int RetweetCount
		{
			get => _retweetCount;
			private set => SetProperty(ref _retweetCount, value);
		}


		public StatusInfo(Status status)
		{
			if (status.RetweetedStatus != null)
			{
				throw new NotSupportedException();
			}

			_id = status.Id;
			_status = status;
		}

		public void Update(Status item)
		{
			if ((item.RetweetedStatus ?? item).Id != _id)
			{
				throw new ArgumentException();
			}

			FavoriteCount = item.FavoriteCount ?? FavoriteCount;
			RetweetCount = item.RetweetCount ?? RetweetCount;
		}

		public bool Equals(StatusInfo other) => Equals(_id, other?.Id);

		public bool Equals(Status other) => Equals(_id, other?.Id);

		public override bool Equals(object obj)
		{
			return (obj is StatusInfo statusInfo && Equals(statusInfo))
				|| (obj is Status status && Equals(status));
		}

		public override int GetHashCode() => _id.GetHashCode();
	}
}
