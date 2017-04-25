using CoreTweet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Liberfy
{
	// Jsonデータにシリアライズする変数にはJsonProperty属性が必要
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	class Account : NotificationObject, IEquatable<Account>, IEquatable<User>
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
			get { return _isValidUserInfo ? Info.ScreenName : _screenName; }
			set { _screenName = value; }
		}

		[JsonProperty]
		public string Name
		{
			get { return _isValidUserInfo ? Info.Name : _name; }
			set { _name = value; }
		}

		[JsonProperty]
		public string ProfileImageUrl
		{
			get { return _isValidUserInfo ? Info.ProfileImageUrl : _profileImageUrl; }
			set { _profileImageUrl = value; }
		}

		[JsonProperty]
		public bool IsProtected
		{
			get { return _isValidUserInfo ? Info.IsProtected : _isProtected; }
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


		private bool _isValidUserInfo;
		private bool _isLoading;

		public bool IsLoading
		{
			get { return _isLoading; }
			set { SetProperty(ref _isLoading, value); }
		}

		public UserInfo Info { get; private set; }

		private Tokens _tokens;
		public Tokens Tokens
		{
			get
			{
				return _tokens ?? (_tokens = Tokens.Create(
				  ConsumerKey, ConsumerSecret,
				  AccessToken, AccessTokenSecret));
			}
		}

		private HashSet<long> _following = new HashSet<long>();
		public HashSet<long> Following => _following;

		private HashSet<long> _follower = new HashSet<long>();
		public HashSet<long> Follower => _follower;

		private HashSet<long> _blocking = new HashSet<long>();
		public HashSet<long> Blocking => _blocking;

		private HashSet<long> _muting = new HashSet<long>();
		public HashSet<long> Muting => _muting;

		private HashSet<long> _incoming = new HashSet<long>();
		public HashSet<long> Incoming => _incoming;

		private HashSet<long> _outgoing = new HashSet<long>();
		public HashSet<long> Outgoing => _outgoing;

		private IdBaseCollection<Reaction> _reactions = new IdBaseCollection<Reaction>();
		public IdBaseCollection<Reaction> Reactions => _reactions;

		public Timeline Timeline { get; private set; }

		[JsonConstructor]
		private Account()
		{
			Info = new UserInfo(this);
			Timeline = new Timeline(this);
		}

		private Account(long id, string name, string screen_name)
		{
			Id = id;
			Name = name;
			ScreenName = screen_name;
		}

		public Account(Tokens tokens) : this(tokens, null) { }

		public Account(Tokens tokens, User user)
		{
			if (user == null)
			{
				Id = tokens.UserId;
				ScreenName = tokens.ScreenName;
				Name = tokens.ScreenName;
				Info = new UserInfo(this);
				_isValidUserInfo = false;
			}
			else
			{
				Id = (long)user.Id;
				Info = new UserInfo(user);
				_isValidUserInfo = true;
			}

			SetTokens(tokens);

			Timeline = new Timeline(this);
		}

		public void SetTokens(Tokens tokens)
		{
			_tokens = new Tokens(tokens);

			ConsumerKey = tokens.ConsumerKey;
			ConsumerSecret = tokens.ConsumerSecret;
			AccessToken = tokens.AccessToken;
			AccessTokenSecret = tokens.AccessTokenSecret;
		}

		private bool _isLoggedIn = false;
		public bool IsLoggedIn
		{
			get => _isLoggedIn;
			set => SetProperty(ref _isLoggedIn, value);
		}

		public bool IsMetadataLoaded { get; set; }

		private AccountLoginStatus _loginStatus;
		public AccountLoginStatus LoginStatus
		{
			get => _loginStatus;
			set => SetProperty(ref _loginStatus, value);
		}

		public bool Login()
		{
			if (IsLoggedIn) return true;

			try
			{
				var user = Tokens.Account.VerifyCredentials(
					include_entities: true,
					skip_status: true,
					include_email: false
				);

				Id = user.Id.Value;

				Info.Update(user);

				IsLoggedIn = true;
				LoginStatus = AccountLoginStatus.Success;

				return true;
			}
			catch (TwitterException tex)
			{
				switch(tex.Status)
				{
					case System.Net.HttpStatusCode.OK:
						break;
				}

				return false;
			}

			return false;
		}

		public static readonly long DummyId = -2;

		public static Account Dummy => new Account(DummyId, "Dummy User", "dummy");

		public override bool Equals(object obj)
		{
			switch (obj)
			{
				case Account account:
					return Equals(account);

				case User user:
					return Equals(user);
			}

			return base.Equals(obj);
		}

		public bool Equals(Account other)
		{
			return other?.Id == Id;
		}

		public bool Equals(User other)
		{
			return other?.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() + "Liberfy.Account".GetHashCode();
		}

		public void LoadMetadata(CancellationToken? cancellationToken = null)
		{
			var tasks = new[]
			{
				Task.Run((Action)LoadFollower),
				Task.Run((Action)LoadFollowing),
				Task.Run((Action)LoadBlock),
				Task.Run((Action)LoadMuting),
				Task.Run((Action)LoadOutgoing),
				Task.Run((Action)LoadIncoming)
			};

			if (cancellationToken.HasValue)
			{
				Task.WaitAll(tasks, cancellationToken.Value);
			}
			else
			{
				Task.WaitAll(tasks);
			}

			tasks = null;
		}

		private string _loadError;
		public string LoadErorr
		{
			get { return _loadError; }
			private set { SetProperty(ref _loadError, value); }
		}

		private void SetLoadError(string name, string message)
		{
			lock (_lockSharedObject)
			{
				var sb = new StringBuilder(_loadError);

				if (sb.Length != 0)
					sb.AppendLine();

				sb.AppendLine($"{name}の取得に失敗しました：");
				sb.AppendLine(message);

				LoadErorr = sb.ToString();
				sb = null;
			}
		}

		public object _lockSharedObject = new object();

		private void LoadIds<T>(Func<EnumerateMode, long, long?, int?, IEnumerable<T>> getMethod, HashSet<T> hashSet, string name)
		{
			try
			{
				hashSet.UnionWith(getMethod(EnumerateMode.Next, Id, null, null));
			}
			catch (Exception ex)
			{
				SetLoadError(name, ex.Message);
			}
		}

		private void LoadIds2<T>(Func<EnumerateMode, long?, IEnumerable<T>> getMethod, HashSet<T> hashSet, string name)
		{
			try
			{
				hashSet.UnionWith(getMethod.Invoke(EnumerateMode.Next, null));
			}
			catch (Exception ex)
			{
				SetLoadError(name, ex.Message);
			}
		}

		private void LoadFollowing()
		{
			LoadIds(_tokens.Friends.EnumerateIds, _following, "フォロー中一覧");
		}

		private void LoadFollower()
		{
			LoadIds(_tokens.Followers.EnumerateIds, _follower, "フォロワー一覧");
		}

		private void LoadBlock()
		{
			LoadIds2(_tokens.Blocks.EnumerateIds, _blocking, "ブロック中一覧");
		}

		private void LoadMuting()
		{
			LoadIds2(_tokens.Mutes.Users.EnumerateIds, _muting, "ミュート中一覧");
		}

		private void LoadIncoming()
		{
			LoadIds2(_tokens.Friendships.EnumerateIncoming, _incoming, "フォロー申請一覧");
		}

		private void LoadOutgoing()
		{
			LoadIds2(_tokens.Friendships.EnumerateOutgoing, _outgoing, "フォロー申請中一覧");
		}

		public void Unload()
		{
			Timeline?.Unload();
			_tokens = null;

			_following.Clear();
			_following = null;

			_follower.Clear();
			_follower = null;

			_blocking.Clear();
			_blocking = null;

			_muting.Clear();
			_muting = null;

			_incoming.Clear();
			_incoming = null;

			_outgoing.Clear();
			_outgoing = null;

			_reactions.Clear();
			_reactions = null;

			_lockSharedObject = null;
		}
	}

	internal enum AccountLoginStatus
	{
		Success,
		NetworkFaliure,
		NotFound,
	}
}
