using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal abstract class SearchColumnOptionBase : ColumnOptionBase
    {
        [DataMember(Name = "search_type")]
        protected string _searchType;
        [IgnoreDataMember]
        public string SearchType
        {
            get => this._searchType;
            set => this.SetProperty(ref this._searchType, value);
        }

        [DataMember(Name = "use_language")]
        protected bool? _useLanguage;
        [IgnoreDataMember]
        public bool UseLanguage
        {
            get => this._useLanguage ?? false;
            set => this.SetProperty(ref this._useLanguage, value);
        }

        [DataMember(Name = "language")]
        protected string _language;
        [IgnoreDataMember]
        public string Language
        {
            get => this._language;
            set => this.SetProperty(ref this._language, value);
        }

        [DataMember(Name = "query")]
        protected string _query;
        [IgnoreDataMember]
        public string Query
        {
            get => this._query;
            set => this.SetProperty(ref _query, value);
        }

        [DataMember(Name = "clear_items_after_search")]
        protected bool? _clearItemsAfterSearch;
        [IgnoreDataMember]
        public bool ClearItemsAfterSearch
        {
            get => this._clearItemsAfterSearch ?? false;
            set => this.SetProperty(ref this._clearItemsAfterSearch, value);
        }

        protected SearchColumnOptionBase ApplyClone(SearchColumnOptionBase option)
        {
            option._searchType = this._searchType;
            option._useLanguage = this._useLanguage;
            option._language = this._language;
            option._query = this._query;
            option._clearItemsAfterSearch = this._clearItemsAfterSearch;

            return option;
        }
    }
}
