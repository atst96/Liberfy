using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMvvmToolkit;

namespace Liberfy
{
    internal class ColumnRemoveCommand : Command<ColumnBase>
    {
        private NotifiableCollection<ColumnBase> _columns;

        public ColumnRemoveCommand() : base(true)
        {
            this._columns = App.Columns;
        }

        protected override bool CanExecute(ColumnBase column)
        {
            return column != null;
        }

        protected override void Execute(ColumnBase column)
        {
            this._columns.Remove(column);
        }
    }
}
