using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Liberfy
{
    internal class ListColumn : StatusColumnBase
    {
        private static string BaseTitle = "List";

        public ListColumn(AccountBase account)
            : base(account, ColumnType.List, BaseTitle)
        {
        }

        private long _listId;
        public long ListId
        {
            get => this._listId;
            set => this.SetProperty(ref this._listId, value);
        }

        public override ColumnSetting GetOption()
        {
            var opt = base.GetOption();

            opt.SetValue("list_id", this._listId);

            return opt;
        }

        protected override void SetOption(ColumnSetting option)
        {
            this.ListId = option.GetValue<long>("list_id");
        }
    }
}
