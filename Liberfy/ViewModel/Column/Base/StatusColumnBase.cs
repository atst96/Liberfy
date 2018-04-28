using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	internal abstract class StatusColumnBase : ColumnBase
	{
	    protected StatusColumnBase(Timeline timeline, ColumnType type, string title = null)
			: base(timeline, type, title)
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
			get { return this._selectedStatus; }
			set { this.SetProperty(ref this._selectedStatus, value); }
		}

		public override void OnShowDetails(IItem item)
		{
			throw new NotImplementedException();
		}
	}
}
