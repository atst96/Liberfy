using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	[Newtonsoft.Json.JsonArray]
	class AccountCollection : FluidCollection<Account>
	{
		public bool ContainsId(double id)
		{
			return this.Any((a) => a.Id == id);
		}

		public Account FromId(long id)
		{
			return this.FirstOrDefault((a) => a.Id == id);
		}
	}
}
