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
		static readonly DataTemplate _emptyTemplate = GetTemplate("EmptyDataTemplate");
		static readonly DataTemplate _searchTemplate = GetTemplate("SearchColumnOptionTemplate");
		static readonly DataTemplate _streamSearchTemplate = GetTemplate("StreamSearchColumnOptionTemplate");

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
