using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Liberfy
{
    internal class ListColumn : StatusColumnBase
    {
        private static string BaseTitle = "List";
        DispatcherTimer _listTimer;

        public ListColumn(Timeline timeline) : base(timeline, ColumnType.List, BaseTitle)
        {
            _listTimer = new DispatcherTimer(
                TimeSpan.FromSeconds(20),
                DispatcherPriority.Normal,
                listTimerTicked, Application.Current.Dispatcher)
            {

            };
        }

        private ListColumnOption _listOption;
        public ListColumnOption ListOption => this._listOption ?? (this._listOption = (ListColumnOption)this.GetOption());

        protected override ColumnOptionBase GetOption()
        {
            return this.InternalColumnOption as ColumnOptionBase ?? (this.InternalColumnOption = new ListColumnOption());
        }

        private void listTimerTicked(object sender, EventArgs e)
        {
            
        }
    }
}
