using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvmToolkit;

namespace Liberfy
{
    internal class ColumnMoveRightCommand : Command<ColumnBase>
    {
        private NotifiableCollection<ColumnBase> _columns;

        public ColumnMoveRightCommand() : base(true)
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
            return column != null && this._columns.IndexOf(column) < (this._columns.Count - 1);
        }

        protected override void Execute(ColumnBase column)
        {
            int idx = this._columns.IndexOf(column);
            this._columns.Move(idx, idx + 1);
        }
    }
}
