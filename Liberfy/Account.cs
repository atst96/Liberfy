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


		[JsonProperty]
		public long Id { get; private set; }

		[JsonProperty]
		public string ScreenName
		{
			get { return _validUserInfo ? Info.ScreenName : _screenName; }
			set { _screenName = value; }
		}

		[JsonProperty]
		public string Name
		{
			get { return _validUserInfo ? Info.Name : _name; }
			set { _name = value; }
		}

		[JsonProperty]
		public string ProfileImageUrl
		{
			get { return _validUserInfo ? Info.ProfileImageUrl : _profileImageUrl; }
			set { _profileImageUrl = value; }
		}

		[JsonProperty]
		public bool IsProtected
		{
			get { return _validUserInfo ? Info.IsProtected : _isProtected; }
			set { _isProtected = value; }
		}

		[JsonProperty]
		public string ConsumerKey { get; private set; }

		[JsonProperty]
		public string ConsumerSecret { get; private set; }

		[JsonProperty]
		public string AccessToken { get; private set; }

		[JsonProperty]
		public string AccessTokenSecret { get; private set; }


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

		private Tokens _tokens;
		[JsonIgnore]
		public Tokens Tokens
		{
			get
			{
				return _tokens ?? (_tokens = Tokens.Create(
				  ConsumerKey, ConsumerSecret,
				  AccessToken, AccessTokenSecret));
			}
		}

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

		private Timeline _timeline;
		[JsonIgnore]
		public Timeline Timeline => _timeline ?? (_timeline = new Timeline(this));

		[Obsolete]
		public Account()
		{
			Info = new UserInfo(this);
		}

		private Account(long id, string name, string screen_name)
		{
			Id = id;
			Name = name;
			ScreenName = screen_name;
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
			Id = (long)user.Id;

			SetTokens(tokens);

			Info = new UserInfo(user);
			_validUserInfo = true;
		}

		public void SetTokens(Tokens tokens)
		{
			_tokens = new Tokens(tokens);

			ConsumerKey = tokens.ConsumerKey;
			ConsumerSecret = tokens.ConsumerSecret;
			AccessToken = tokens.AccessToken;
			AccessTokenSecret = tokens.AccessTokenSecret;
		}

		[JsonIgnore]
		public bool IsLoggedIn { get; set; }

		[JsonIgnore]
		public bool IsMetadataLoaded { get; set; }

		public bool Login()
		{
			if (IsLoggedIn) return true;

			try
			{
				var user = Tokens.Account.VerifyCredentials();

				Info.Update(user);
				Id = user.Id.Value;

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public static readonly long DummyId = -2;

		public static Account Dummy => new Account(DummyId, "Dummy User", "dummy");
	}
}
