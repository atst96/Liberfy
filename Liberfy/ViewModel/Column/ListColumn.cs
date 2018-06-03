using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Liberfy
{
    internal class ListColumn : StatusColumnBase<ListColumnOption>
    {
        private static string BaseTitle = "List";
        DispatcherTimer _listTimer;

        public ListColumn(TwitterTimeline timeline) : base(timeline, ColumnType.List, BaseTitle)
        {
            _listTimer = new DispatcherTimer(
                TimeSpan.FromSeconds(20),
                DispatcherPriority.Normal,
                listTimerTicked, Application.Current.Dispatcher)
            {

            };
        }

        protected override ListColumnOption CreateOption() => new ListColumnOption();

        private void listTimerTicked(object sender, EventArgs e)
        {
            
        }
    }
}
