using System;

namespace Liberfy.Data.Settings.Columns
{
    internal abstract class StatusColumnBase : ColumnBase
    {
        protected StatusColumnBase(IAccount account, ColumnType type, string title = null)
            : base(account, type, title)
        {
            if (type == ColumnType.Status || !type.HasFlag(ColumnType.Status))
            {
                throw new NotSupportedException();
            }
        }

        public override bool IsStatusColumn { get; } = true;

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
