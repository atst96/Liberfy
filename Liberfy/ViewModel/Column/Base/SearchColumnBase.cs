using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal abstract class SearchColumnBase<T> : StatusColumnBase<T>
        where T: ColumnOptionBase
    {
        protected SearchColumnBase(Timeline timeline, ColumnType type) : base(timeline, type)
        {
        }

        private static string BaseTitle = "Search";

        //private string _searchType;
        //public string SearchType
        //{
        //	get { return GetPropValue(ref _searchType, "search_type", "tweet"); }
        //	set { SetValueWithProp(ref _searchType, value, "search_type"); }
        //}

        //private bool? _useLanguage;
        //public bool UseLanguage
        //{
        //	get { return GetPropValue(ref _useLanguage, "use_language", false); }
        //	set { SetValueWithProp(ref _useLanguage, value, "use_language"); }
        //}

        //private string _language;
        //public string Language
        //{
        //	get { return GetPropValue(ref _language, "language", "ja"); }
        //	set { SetValueWithProp(ref _language, value, "language"); }
        //}

        //private string _query;
        //public string Query
        //{
        //	get { return GetPropValue(ref _query, "query", ""); }
        //	set
        //	{
        //		if(SetValueWithProp(ref _query, value, "query"))
        //		{
        //			Title = string.IsNullOrEmpty(value)
        //				? BaseTitle : $"{BaseTitle}: {value}";
        //		}
        //	}
        //}

        //private bool? _clearItemsAfterSearch;
        //public bool ClearItemsAfterSearch
        //{
        //	get { return GetPropValue(ref _clearItemsAfterSearch, "clear_items_after_search", false); }
        //	set { SetValueWithProp(ref _clearItemsAfterSearch, value, "clear_items_after_search"); }
        //}
    }
}
