using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal abstract class SearchColumnBase : StatusColumnBase
    {
        protected SearchColumnBase(TwitterTimeline timeline, ColumnType type)
            : base(timeline, type)
        {
        }

        private static string BaseTitle = "Search";

        public override ColumnSetting GetOption()
        {
            var option = base.GetOption();

            option.SetValue("search_type", this._searchType);

            option.SetValue("use_language", this._useLanguage);
            option.SetValue("language", this._language);

            option.SetValue("query", this._query);

            option.SetValue("clear_items", this._clearItemsAfterSearch);

            return option;
        }

        protected override void SetOption(ColumnSetting option)
        {
            base.SetOption(option);

            this._searchType = option.GetValue<string>("search_type");

            this._useLanguage = option.GetValue<bool>("use_language");
            this.Language = option.GetValue<string>("language");

            this._clearItemsAfterSearch = option.GetValue<bool>("clear_items");
        }

        private string _searchType = "tweet";
        public string SearchType
        {
            get => this._searchType;
            set => this.SetProperty(ref this._searchType, value);
        }

        private bool _useLanguage;
        public bool UseLanguage
        {
            get => this._useLanguage;
            set => this.SetProperty(ref this._useLanguage, value);
        }

        private string _language;
        public string Language
        {
            get => this._language;
            set => this.SetProperty(ref this._language, value);
        }

        private string _query;
        public string Query
        {
            get => this._query;
            set => this.SetProperty(ref this._query, value);
        }

        private bool _clearItemsAfterSearch;
        public bool ClearItemsAfterSearch
        {
            get => this._clearItemsAfterSearch;
            set => this.SetProperty(ref this._clearItemsAfterSearch, value);
        }
    }
}
