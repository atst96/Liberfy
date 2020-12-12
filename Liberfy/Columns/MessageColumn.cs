using System;
using Liberfy.Data.Settings.Columns;

namespace Liberfy
{
    internal class MessageColumn : ColumnBase
    {
        public MessageColumn(IAccount account)
            : base(account, ColumnType.Messages, "Message")
        {
        }

        public override IColumnSetting GetSetting()
        {
            throw new NotImplementedException();
        }

        public override void OnShowDetails(IItem item) { }
    }
}
