using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
	internal class TimelineItemTemplateSelector : DataTemplateSelector
	{
		private static DataTemplate _statusItemTemplate = Application.Current.TryFindResource("StatusItemTemplate") as DataTemplate;

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if(item is StatusItem)
			{
				return _statusItemTemplate;
			}

			return base.SelectTemplate(item, container);
		}
	}
}
