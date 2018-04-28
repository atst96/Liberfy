﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class HomeColumn : StatusColumnBase<GeneralColumnOption>
    {
        public HomeColumn(Timeline timeline)
            : base(timeline, ColumnType.Home, "Home")
        {
            if (timeline != null)
            {
                timeline.OnHomeStatusesLoaded += OnHomeTimelineLoaded;
            }
        }

        protected override GeneralColumnOption CreateOption() => new GeneralColumnOption(this.Type);

        private void OnHomeTimelineLoaded(object sender, IEnumerable<StatusItem> e)
        {
            if (sender is Timeline timeline)
                timeline.OnHomeStatusesLoaded -= OnHomeTimelineLoaded;

            this.Items.Reset(e);
        }
    }
}