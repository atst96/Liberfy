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

		public Timeline(Account account)
		{
			this.account = account;
		}
	}
}
