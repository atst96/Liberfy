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

		public Timeline(Account account)
		{
			_account = account;
			_userId = account.Id;
		}

		public void Unload()
		{
		}
	}
}
