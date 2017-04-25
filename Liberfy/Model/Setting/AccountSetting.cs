using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class AccountSetting : SettingBase
	{
		public bool ContainsId(double id)
		{
			return Accounts.Any((a) => a.Id == id);
		}

		public Account FromId(long id)
		{
			return Accounts.FirstOrDefault((a) => a.Id == id);
		}

		private FluidCollection<Account> _accounts =
			new FluidCollection<Account>();

		[JsonProperty("accounts")]
		public FluidCollection<Account> Accounts
		{
			get { return _accounts; }
			set
			{
				if (value != null)
					_accounts.Reset(value);
				else
					_accounts.Reset();
			}
		}
	}
}
