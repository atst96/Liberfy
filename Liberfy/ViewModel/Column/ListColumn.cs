using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Liberfy
{
	class ListColumn : StatusColumn
	{
		private static string BaseTitle = "List";
		DispatcherTimer _listTimer;

		public ListColumn(Account account)
			: base(account, ColumnType.List, BaseTitle)
		{
			_listTimer = new DispatcherTimer(
				TimeSpan.FromSeconds(20),
				DispatcherPriority.Normal,
				listTimerTicked, Application.Current.Dispatcher)
			{

			};
		}

		private long? _listId;
		public long ListId
		{
			get { return GetPropValue(ref _listId, "list_id", -1); }
			set { SetValueWithProp(ref _listId, value, "list_id"); }
		}

		private void listTimerTicked(object sender, EventArgs e)
		{
			
		}
	}
}
