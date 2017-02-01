using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class StatusColumn : StatusColumnBase
	{
		public StatusColumn(Account account, ColumnType type, string title = null)
			: base(account, type, title)
		{
			
		}
	}
}
