using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    class MessageColumn : ColumnBase<GeneralColumnOption>
    {
        public MessageColumn(TwitterTimeline timeline)
            : base(timeline, ColumnType.Messages, "Message")
        {
        }

        protected override GeneralColumnOption CreateOption() => new GeneralColumnOption(this.Type);

        public override void OnShowDetails(IItem item) { }
    }
}
