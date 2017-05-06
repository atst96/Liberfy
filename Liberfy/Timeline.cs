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
		private FluidCollection<ColumnBase> Columns => App.Columns;

		private readonly long _userId;

		public Timeline(Account account)
		{
			_account = account;
			_userId = account.Id;
		}

		public void Unload()
		{
			var columns = Columns;

			foreach (var column in columns
				.Where(c => c.Account.Id == _userId).ToArray())
			{
				columns.Remove(column);
			}
		}
	}
}
