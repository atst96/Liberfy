using CoreTweet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	[JsonObject]
	class Account : NotificationObject
	{
		private string _screenName;
		private string _name;
		private string _profileImageUrl;
		private bool _isProtected;


		[JsonProperty("Id")]
		public long Id { get; private set; }

		[JsonProperty("ScreenName")]
		public string ScreenName
		{
			get { return _validUserInfo ? Info.ScreenName : _screenName; }
			set { _screenName = value; }
		}

		[JsonProperty("Name")]
		public string Name
		{
			get { return _validUserInfo ? Info.Name : _name; }
			set { _name = value; }
		}

		[JsonProperty("ProfileImageUrl")]
		public string ProfileImageUrl
		{
			get { return _validUserInfo ? Info.ProfileImageUrl : _profileImageUrl; }
			set { _profileImageUrl = value; }
		}

		[JsonProperty("IsProtected")]
		public bool IsProtected
		{
			get { return _validUserInfo ? Info.IsProtected : _isProtected; }
			set { _isProtected = value; }
		}

		[JsonProperty]
		public string ConsumerKey { get; set; }

		[JsonProperty]
		public string ConsumerSecret { get; set; }

		[JsonProperty]
		public string AccessToen { get; set; }

		[JsonProperty]
		public string AccessTokenSecret { get; set; }


		private bool _validUserInfo;
		private bool _isLoading;
		[JsonIgnore]
		public bool IsLoading
		{
			get { return _isLoading; }
			set { SetProperty(ref _isLoading, value); }
		}

		[JsonIgnore]
		public UserInfo Info { get; private set; }


		[JsonIgnore]
		public HashSet<long> Following { get; } = new HashSet<long>();

		[JsonIgnore]
		public HashSet<long> Follower { get; } = new HashSet<long>();

		[JsonIgnore]
		public HashSet<long> Blocking { get; } = new HashSet<long>();

		[JsonIgnore]
		public HashSet<long> Muting { get; } = new HashSet<long>();

		[JsonIgnore]
		public HashSet<long> Incoming { get; } = new HashSet<long>();

		[JsonIgnore]
		public HashSet<long> Outgoing { get; } = new HashSet<long>();

		[JsonIgnore]
		public IdBaseCollection<Reaction> Reactions { get; } = new IdBaseCollection<Reaction>();


		[Obsolete]
		public Account()
		{
			Info = new UserInfo(this);
		}

		public void SetTokens(Tokens tokens)
		{
			ConsumerKey = tokens.ConsumerKey;
			ConsumerSecret = tokens.ConsumerSecret;
			AccessToen = tokens.AccessToken;
			AccessTokenSecret = tokens.AccessTokenSecret;
		}

		public Account(Tokens tokens)
		{
			SetTokens(tokens);

			Id = tokens.UserId;
			ScreenName = tokens.ScreenName;
			Name = tokens.ScreenName;

			Info = new UserInfo(this);
		}

		public Account(Tokens tokens, User user)
		{
			SetTokens(tokens);
			Info = new UserInfo(this);

			SetUser(user);
		}

		public void SetUser(User user)
		{
			Id = user.Id ?? Id;

			Info.Update(user);

			_validUserInfo = true;
		}
	}
}
