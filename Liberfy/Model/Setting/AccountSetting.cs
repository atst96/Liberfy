using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	[JsonObject(memberSerialization: MemberSerialization.OptIn)]
	internal class AccountSetting : NotificationObject
	{
		public bool ContainsId(double id)
		{
			return Accounts.Any((a) => a.Id == id);
		}

		public Account FromId(long id)
		{
			return Accounts.FirstOrDefault((a) => a.Id == id);
		}

		[JsonProperty("accounts")]
		private Account[] _jAccounts
		{
			get => Accounts.ToArray();
			set
			{
				if (value != null)
					Accounts.Reset(value);
				else
					Accounts.Reset();
			}
		}

		public FluidCollection<Account> Accounts { get; } = new FluidCollection<Account>();
	}
}
