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
		private Account _account;
		private Tokens _tokens => _account.Tokens;

		private readonly long _userId;

		private HashSet<long> _following => _account.Following;
		private HashSet<long> _follower => _account.Follower;
		private HashSet<long> _blocking => _account.Blocking;
		private HashSet<long> _muting => _account.Muting;
		private HashSet<long> _outgoing => _account.Outgoing;
		private HashSet<long> _incoming => _account.Incoming;

		public Timeline(Account account)
		{
			_account = account;
			_userId = account.Id;
		}

		public void Unload()
		{
			throw new NotImplementedException();
		}
	}
}
