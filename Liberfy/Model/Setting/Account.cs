using CoreTweet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class Account : NotificationObject
	{
		[JsonProperty("Id")]
		public long Id { get; private set; }

		private string _screenName;
		[JsonProperty("ScreenName")]
		public string ScreenName
		{
			get { return Info?.ScreenName ?? _screenName; }
			set { _screenName = value; }
		}

		private string _name;
		[JsonProperty("Name")]
		public string Name
		{
			get { return Info?.Name ?? _name; }
			set { _name = value; }
		}

		private string _profileImageUrl;
		[JsonProperty("ProfileImageUrl")]
		public string ProfileImageUrl
		{
			get { return Info?.ProfileImageUrl ?? _profileImageUrl; }
			set { _profileImageUrl = value; }
		}

		private bool _isProtected;
		[JsonProperty("IsProtected")]
		public bool IsProtected
		{
			get { return Info?.IsProtected ?? _isProtected; }
			set { _isProtected = value; }
		}


		[JsonProperty("Token")]
		public Tokens Tokens { get; private set; }

		[JsonProperty("OfficialToken")]
		public Tokens OfficialTokens { get; private set; }

		private bool _validUserInfo;
		private UserInfo _info;
		public UserInfo Info
		{
			get { return _validUserInfo ? _info : (_info ?? (_info = new UserInfo(this))); }
		}

		private HashSet<long>
			_following,_follower, 
			_blocking, _muting,
			_incoming, _outgoing;

		public HashSet<long> Following => _following ?? (_following = new HashSet<long>());

		public HashSet<long> Follower => _follower ?? (_follower = new HashSet<long>());

		public HashSet<long> Blocking => _blocking ?? (_blocking = new HashSet<long>());

		public HashSet<long> Muting => _muting ?? (_muting = new HashSet<long>());

		public HashSet<long> Incoming => _incoming ?? (_incoming = new HashSet<long>());

		public HashSet<long> Outgoing => _outgoing ?? (_outgoing = new HashSet<long>());

		public IdBaseCollection<Reaction> Reactions { get; } = new IdBaseCollection<Reaction>();

		public Account(Tokens tokens, User user)
		{
			Tokens = tokens;

			SetUser(user);
		}

		public void SetUser(User user)
		{
			Id = user.Id ?? Id;
			SetProperty(ref _info, DataStore.UserAddOrUpdate(user), nameof(Info));
			_validUserInfo = true;
		}
	}
}
