using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class Timeline : NotificationObject
	{
		private Account account;
		private Tokens tokens => account.Tokens;

		private long userId;

		private HashSet<long> _following;
		private HashSet<long> _follower;
		private HashSet<long> _blocking;
		private HashSet<long> _muting;
		private HashSet<long> _outgoing;
		private HashSet<long> _incoming;

		public Timeline(Account account)
		{
			this.account = account;
			this.userId = account.Id;

			this._following = account.Following;
			this._follower = account.Follower;
			this._blocking = account.Blocking;
			this._muting = account.Muting;
			this._outgoing = account.Outgoing;
			this._incoming = account.Incoming;
		}

		public void LoadAccount()
		{
			if (!account.Login()) return;

			Task.WaitAll(
				Task.Run(() =>
				{
					try
					{
						_following.UnionWith(tokens
							.Friends.EnumerateIds(EnumerateMode.Next, userId));
					}
					catch (Exception ex)
					{

					}
				}),
				Task.Run(() =>
				{
					try
					{
						_follower.UnionWith(tokens
							.Followers.EnumerateIds(EnumerateMode.Next, userId));
					}
					catch (Exception ex)
					{

					}
				}),
				Task.Run(() =>
				{
					try
					{
						_blocking.UnionWith(tokens
							.Blocks.EnumerateIds(EnumerateMode.Next));
					}
					catch (Exception ex)
					{

					}
				}),
				Task.Run(() =>
				{
					try
					{
						_muting.UnionWith(tokens
							.Mutes.Users.EnumerateIds(EnumerateMode.Next));
					}
					catch (Exception ex)
					{

					}
				}),
				Task.Run(() =>
				{
					try
					{
						_outgoing.UnionWith(tokens
							.Friendships.EnumerateOutgoing(EnumerateMode.Next));
					}
					catch (Exception ex)
					{

					}
				}),
				Task.Run(() =>
				{
					try
					{
						if (account.IsProtected)
						{
							_incoming.UnionWith(tokens
								.Friendships.EnumerateIncoming(EnumerateMode.Next));
						}
					}
					catch (Exception ex)
					{

					}
				}));
		}

	}
}
