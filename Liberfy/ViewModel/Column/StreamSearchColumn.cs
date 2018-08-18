using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    class StreamSearchColumn : SearchColumnBase
    {
        public StreamSearchColumn(AccountBase account)
            : base(account, ColumnType.Stream)
        {
        }
    }
}
