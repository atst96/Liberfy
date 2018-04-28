using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
	class ColumnOptionTemplateSelector : DataTemplateSelector
	{
		private static readonly DataTemplate _emptyTemplate = GetTemplate("EmptyDataTemplate");
		private static readonly DataTemplate _searchTemplate = GetTemplate("SearchColumnOptionTemplate");
		private static readonly DataTemplate _streamSearchTemplate = GetTemplate("StreamSearchColumnOptionTemplate");

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			switch((item as IColumn)?.Type)
			{
				case ColumnType.Search:
					return _searchTemplate;

				case ColumnType.Stream:
					return _streamSearchTemplate;

				default:
					return _emptyTemplate;
			}
		}

		protected static DataTemplate GetTemplate(object resourceKey)
		{
			return App.Current.TryFindResource(resourceKey) as DataTemplate;
		}
	}
}
