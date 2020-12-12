using System;
using Liberfy.Data.Settings.Columns;

namespace Liberfy
{
    internal class ListColumn : StatusColumnBase
    {
        private static string BaseTitle = "List";

        public ListColumn(IAccount account)
            : base(account, ColumnType.List, BaseTitle)
        {
        }

        private long _listId;
        public long ListId
        {
            get => this._listId;
            set => this.SetProperty(ref this._listId, value);
        }

        public override IColumnSetting GetSetting()
        {
            throw new NotImplementedException();

            //return new ListColumnSetting
            //{
            //    ItemId = this.Account.ItemId,
            //    ListId = this._listId,
            //};
        }

        protected override void SetOption(IColumnSetting option)
        {
            throw new NotImplementedException();
            // this.ListId = option.GetValue<long>("list_id");
        }
    }
}
