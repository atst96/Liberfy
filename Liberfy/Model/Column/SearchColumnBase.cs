using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class SearchColumnBase : StatusColumnBase
	{
		protected SearchColumnBase(Account account, ColumnType type)
			: base(account, type)
		{
		}

		private static string BaseTitle = "Search";

		private string _searchType;
		public string SearchType
		{
			get { return _searchType; }
			set { SetProperty(ref _searchType, value); }
		}

		private bool _useLanguage;
		public bool UseLanguage
		{
			get { return _useLanguage; }
			set { SetProperty(ref _useLanguage, value); }
		}

		private string _language = "ja";
		public string Language
		{
			get { return _language; }
			set { SetProperty(ref _language, value); }
		}

		private string _query;
		public string Query
		{
			get { return _query; }
			set
			{
				if (SetProperty(ref _query, value))
				{
					Title = string.IsNullOrEmpty(value)
						? BaseTitle : $"{BaseTitle}: {value}";
				}
			}
		}

		public bool ClearItemsAfterSearch { get; set; }

		protected override ColumnProperties CreateProperties()
		{
			return new ColumnProperties
			{
				["clear_items_after_search"] = ClearItemsAfterSearch,
				["search_type"] = _searchType,
				["use_language"] = _useLanguage,
				["language"] = _language,
				["query"] = _query,
			};
		}

		protected override void ApplyProperties(ColumnProperties prop)
		{
			SearchType = prop.TryGetValue<string>("search_type");
			UseLanguage = prop.TryGetValue<bool>("use_language");
			Language = prop.TryGetValue<string>("language");
			Query = prop.TryGetValue<string>("query");

			ClearItemsAfterSearch = prop.TryGetValue<bool>("clear_items_after_search");
		}
	}
}
