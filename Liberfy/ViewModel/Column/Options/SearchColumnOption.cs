using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.ViewModel.Column.Options
{
    [DataContract]
    internal class SearchColumnOption : SearchColumnOptionBase
    {
        public override ColumnType Type { get; } = ColumnType.Search;

        [DataMember(Name = "use_result_type")]
        private bool? _useResultType;
        [IgnoreDataMember]
        public bool UseResultType
        {
            get => this._useResultType ?? false;
            set => this.SetProperty(ref this._useResultType, value);
        }

        [DataMember(Name = "result_type")]
        private string _resultType;
        [IgnoreDataMember]
        public string ResultType
        {
            get => this._resultType;
            set => this.SetProperty(ref this._resultType, value);
        }

        [DataMember(Name = "use_until")]
        private bool? _useUntil;
        [IgnoreDataMember]
        public bool UseUntil
        {
            get => this._useUntil ?? false;
            set => this.SetProperty(ref this._useUntil, value);
        }

        [DataMember(Name = "until")]
        private string _until;
        [IgnoreDataMember]
        public string Until
        {
            get => this._until;
            set => this.SetProperty(ref this._until, value);
        }

        [DataMember(Name = "use_since_id")]
        private bool? _useSinceId;
        [IgnoreDataMember]
        public bool UseSince
        {
            get => this._useSinceId ?? false;
            set => this.SetProperty(ref this._useSinceId, value);
        }

        [DataMember(Name = "since_id")]
        private long? _sinceId;
        [IgnoreDataMember]
        public long Since
        {
            get => this._sinceId ?? default(long);
            set => this.SetProperty(ref this._sinceId, value);
        }

        [DataMember(Name = "use_max_id")]
        private bool? _useMaxId;
        [IgnoreDataMember]
        public bool UseMaxId
        {
            get => this._useMaxId ?? false;
            set => this.SetProperty(ref this._useMaxId, value);
        }

        [DataMember(Name = "max_id")]
        private long? _maxId;
        [IgnoreDataMember]
        public long MaxId
        {
            get => this._maxId ?? default(long);
            set => this.SetProperty(ref this._maxId, value);
        }

        public override ColumnOptionBase Clone()
        {
            return this.ApplyClone(
                new SearchColumnOption
                {
                    _useResultType = this._useResultType,
                    _resultType = this._resultType,
                    _useUntil = this._useUntil,
                    _until = this._until,
                    _useSinceId = this._useSinceId,
                    _sinceId = this._sinceId,
                    _useMaxId = this._useMaxId,
                    _maxId = this._maxId,
                }
            );
        }
    }
}
