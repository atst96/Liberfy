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

		public long ListId { get; set; }

		private void listTimerTicked(object sender, EventArgs e)
		{
			
		}

		protected override ColumnProperties CreateProperties()
			=> new ColumnProperties(base.CreateProperties())
			{
				["list_id"] = ListId,
			};

		protected override void ApplyProperties(ColumnProperties prop)
		{
			base.ApplyProperties(prop);

			ListId = prop.TryGetValue<long>("list_id");
		}
	}
}
