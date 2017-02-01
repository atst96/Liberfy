using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	abstract class StatusColumnBase : ColumnBase
	{
		protected StatusColumnBase(Account account, ColumnType type, string title = null)
			: base(account, type, title)
		{
			if (type == ColumnType.Status || !type.HasFlag(ColumnType.Status))
				throw new NotSupportedException();
		}

		public override bool IsStatusColumn { get; } = true;

		public List<StatusItem> BeforeConversation { get; protected set; }
		public List<StatusItem> AfterConversation { get; protected set; }

		private StatusItem _selectedStatus;
		public StatusItem SelectedStatus
		{
			get { return _selectedStatus; }
			set { SetProperty(ref _selectedStatus, value); }
		}

		public override void OnShowDetails(IItem item)
		{
			throw new NotImplementedException();
		}
	}
}
