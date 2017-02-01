using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class MessageColumn : ColumnBase
	{
		public MessageColumn(Account account)
			: base(account, ColumnType.Messages, "Message")
		{
		}

		public override void OnShowDetails(IItem item) { }
	}
}
