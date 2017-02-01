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
		static DataTemplate _emptyTemplate { get; } = GetTemplate("EmptyDataTemplate");
		static DataTemplate _searchTemplate { get; } = GetTemplate("SearchColumnOptionTemplate");
		static DataTemplate _streamSearchTemplate { get; } = GetTemplate("StreamSearchColumnOptionTemplate");

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			switch((item as ColumnBase)?.Type)
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
