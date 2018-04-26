using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.ViewModel.Column.Options
{
    [DataContract]
    internal class ListColumnOption : ColumnOptionBase
    {
        public override ColumnType Type { get; } = ColumnType.List;

        [DataMember(Name = "list_id")]
        private long? _listId;
        [IgnoreDataMember]
        public long ListId
        {
            get => this._listId ?? -1;
            set => this.SetProperty(ref this._listId, value);
        }

        public override ColumnOptionBase Clone() => new ListColumnOption
        {
            _listId = this._listId,
        };
    }
}
