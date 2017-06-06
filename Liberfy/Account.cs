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
	[JsonObject(MemberSerialization.OptIn)]
	internal class Account : NotificationObject, IEquatable<Account>, IEquatable<User>
	{
		private static FluidCollection<ColumnBase> Columns => App.Columns;
		private bool _isLoading;


		#region JsonSettings

		#region Account info

		public long Id { get; private set; }

		#endregion

		#region Account settings

		private bool _automaticallyLogin = true;
		public bool AutomaticallyLogin
		{
			get => _automaticallyLogin;
			set => SetProperty(ref _automaticallyLogin, value);
		}

		private bool _automaticallyLoadTimeline = true;
		public bool AutomaticallyLoadTimeline
		{
			get => _automaticallyLoadTimeline;
			set => SetProperty(ref _automaticallyLoadTimeline, value);
		}

		#endregion

		#endregion


		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(ref _isLoading, value);
		}

		public UserInfo Info { get; private set; }

		public Tokens Tokens { get; private set; }

		private SortedSet<long> _following = new SortedSet<long>();

		private SortedSet<long> _follower = new SortedSet<long>();

		private SortedSet<long> _blocking = new SortedSet<long>();

		private SortedSet<long> _muting = new SortedSet<long>();

		private SortedSet<long> _incoming = new SortedSet<long>();

		private SortedSet<long> _outgoing = new SortedSet<long>();

		private SortedDictionary<long, StatusReaction> _statusReactions = new SortedDictionary<long, StatusReaction>();

		public StatusReaction GetStatusReaction(long user_id)
		{
			if (!_statusReactions.TryGetValue(user_id, out StatusReaction r))
			{
				r = new StatusReaction();
				_statusReactions.Add(user_id, r);
			}

			return r;
		}

		public Timeline Timeline { get; }

		private Account(long id, string name, string screenname)
		{
			Id = id;
			Info = new UserInfo(id, name, screenname, false, null);
			Timeline = new Timeline(this);
		}

		public Account(AccountItem item)
		{
			if (item == null)
				throw new Exception();

			Id = item.Id;

			Info = DataStore.Users.GetOrAdd(
				item.Id,
				_ => new UserInfo(item.Id, item.Name, item.ScreenName, item.IsProtected, item.ProfileImageUrl));

			SetTokens(item.Token.ToCoreTweetTokens());
			Timeline = new Timeline(this);
		}

		public Account(Tokens tokens) : this(tokens, null) { }

		public Account(Tokens tokens, User user)
		{
			if (user == null)
			{
				Id = tokens.UserId;
				Info = DataStore.Users.GetOrAdd(
					Id, _ => new UserInfo(Id, tokens.ScreenName, tokens.ScreenName, false, null));
			}
			else
			{
				Id = (long)user.Id;
				Info = new UserInfo(user);
				Info = DataStore.Users.AddOrUpdate(
					Id, _ => new UserInfo(user),
					(id, info) =>
					{
						info.Update(user);
						return info;
					});
			}

			SetTokens(tokens);
			Timeline = new Timeline(this);
		}

		public void SetTokens(Tokens tokens)
		{
			Tokens = new Tokens(tokens);
		}

		private bool _isLoggedIn;
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

		public Task<bool> LoginAsync() => Task.Run((Func<bool>)Login);

		public bool Login()
		{
			if (_isLoggedIn) return true;

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
				switch (tex.Status)
				{
					case System.Net.HttpStatusCode.OK:
						break;
				}
			}

			return false;
		}

		public const long DummyId = -2;

		public static Account Dummy => new Account(DummyId, "Dummy User", "dummy");

		public override bool Equals(object obj)
		{
			return (obj is Account account && Equals(account))
				|| (obj is User user && Equals(user));
		}

		public bool Equals(Account other) => other?.Id == Id;

		public bool Equals(User other) => other?.Id == Id;

		public override int GetHashCode()
		{
			return Id.GetHashCode() + "Liberfy.Account".GetHashCode();
		}

		public Task LoadDetails() => Task.WhenAll(
			Task.Run((Action)LoadFollower),
			Task.Run((Action)LoadFollowing),
			Task.Run((Action)LoadBlock),
			Task.Run((Action)LoadMuting),
			Task.Run((Action)LoadOutgoing),
			Task.Run((Action)LoadIncoming)
		);

		public void StartTimeline()
		{
			Timeline.Load();
		}

		private string _loadError;
		public string LoadErorr
		{
			get => _loadError;
			private set => SetProperty(ref _loadError, value);
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

		#region LoadDetailsメソッド郡

		private void LoadFollowing()
		{
			GetIds((DelgEnumIds4<long>)Tokens.Friends.EnumerateIds, _following, "フォロー中一覧");
		}

		private void LoadFollower()
		{
			GetIds((DelgEnumIds4<long>)Tokens.Followers.EnumerateIds, _follower, "フォロワー一覧");
		}

		private void LoadBlock()
		{
			GetIds(Tokens.Blocks.EnumerateIds, _blocking, "ブロック中一覧");
		}

		private void LoadMuting()
		{
			GetIds(Tokens.Mutes.Users.EnumerateIds, _muting, "ミュート中一覧");
		}

		private void LoadIncoming()
		{
			GetIds(Tokens.Friendships.EnumerateIncoming, _incoming, "フォロー申請一覧");
		}

		private void LoadOutgoing()
		{
			GetIds(Tokens.Friendships.EnumerateOutgoing, _outgoing, "フォロー申請中一覧");
		}

		private delegate IEnumerable<T> DelgEnumIds2<T>(EnumerateMode mode, long? cursor = null);
		private delegate IEnumerable<T> DelgEnumIds4<T>(EnumerateMode mode, long user_id, long? cursor = null, int? count = null);

		private void GetIds<T>(DelgEnumIds4<T> enumerateIds, SortedSet<T> set, string name)
		{
			try
			{
				set.UnionWith(enumerateIds(EnumerateMode.Next, Id));
			}
			catch (Exception ex)
			{
				SetLoadError(name, ex.Message);
			}
		}

		private void GetIds<T>(DelgEnumIds2<T> enumerateIds, SortedSet<T> set, string name)
		{
			try
			{
				set.UnionWith(enumerateIds(EnumerateMode.Next));
			}
			catch (Exception ex)
			{
				SetLoadError(name, ex.Message);
			}
		}

		#endregion LoadDetails

		public void Unload()
		{
			Timeline.Unload();
			Tokens = null;

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

			_statusReactions.Clear();
			_statusReactions = null;

			_lockSharedObject = null;
		}

		public AccountItem ToSetting() => new AccountItem
		{
			Id                        = this.Id,
			Name                      = Info.Name,
			ScreenName                = Info.ScreenName,
			IsProtected               = Info.IsProtected,
			ProfileImageUrl           = Info.ProfileImageUrl,
			Token                     = ApiTokenInfo.FromCoreTweetTokens(Tokens),
			AutomaticallyLogin        = this.AutomaticallyLogin,
			AutomaticallyLoadTimeline = this.AutomaticallyLoadTimeline
		};
	}

	internal enum AccountLoginStatus
	{
		Success,
		NetworkFaliure,
		NotFound,
	}
}
