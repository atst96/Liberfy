using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Data.Settings.Columns;

namespace Liberfy
{
    internal abstract class SearchColumnBase : StatusColumnBase
    {
        protected SearchColumnBase(IAccount account, ColumnType type)
            : base(account, type)
        {
        }

        private static string BaseTitle = "Search";

        public override IColumnSetting GetSetting()
        {
            throw new NotImplementedException();

            //return new SearchColumnSetting
            //{
            //    UserId = this.Account.ItemId,
            //    Search_type = this._searchType),
            //    Use_language = this._useLanguage),
            //    Language = this._language),
            //    Query = this._query),
            //    ClearItems = this._clearItemsAfterSearch),
            //};
        }

        protected override void SetOption(IColumnSetting option)
        {
            throw new NotImplementedException();

            //base.SetOption(option);

            //this._searchType = option.GetValue<string>("search_type");

            //this._useLanguage = option.GetValue<bool>("use_language");
            //this.Language = option.GetValue<string>("language");

            //this._clearItemsAfterSearch = option.GetValue<bool>("clear_items");
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
