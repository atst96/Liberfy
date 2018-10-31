using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class ColumnMoveLeftCommand : Command<ColumnBase>
    {
        private NotifiableCollection<ColumnBase> _columns;

        public ColumnMoveLeftCommand() : base(true)
        {
            this._columns = TimelineBase.Columns;
            this._columns.CollectionChanged += this.ColumnsChanged;
        }

        private void ColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaiseCanExecute();
        }

        protected override bool CanExecute(ColumnBase column)
        {
            return column != null && this._columns.IndexOf(column) > 0;
        }

        protected override void Execute(ColumnBase column)
        {
            int idx = this._columns.IndexOf(column);
            this._columns.Move(idx, idx - 1);
        }
    }
}
