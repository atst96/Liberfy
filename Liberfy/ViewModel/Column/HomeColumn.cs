using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class HomeColumn : StatusColumnBase
    {
        public HomeColumn(AccountBase account)
            : base(account, ColumnType.Home, "Home")
        {
        }
    }
}
