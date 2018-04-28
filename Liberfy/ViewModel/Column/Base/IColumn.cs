using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal interface IColumn
    {
        ColumnType Type { get; }

        ColumnOptionBase GetOption();
        void SetOption(ColumnOptionBase column);
    }
}
