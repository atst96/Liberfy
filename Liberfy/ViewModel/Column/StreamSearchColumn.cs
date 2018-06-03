using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    class StreamSearchColumn : SearchColumnBase<GeneralColumnOption>
    {
        public StreamSearchColumn(TwitterTimeline timeline) : base(timeline, ColumnType.Stream)
        {
        }

        protected override GeneralColumnOption CreateOption() => new GeneralColumnOption(this.Type);
    }
}
