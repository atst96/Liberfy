using System.Runtime.Serialization;

namespace Liberfy.ViewModel.Column.Options
{
    [DataContract]
    internal class GeneralColumnOption : ColumnOptionBase
    {
        [DataMember(Name = "type")]
        protected ColumnType _type;

        [IgnoreDataMember]
        public override ColumnType Type => this._type;

        [Utf8Json.SerializationConstructor]
        public GeneralColumnOption() { }

        public GeneralColumnOption(ColumnType type)
        {
            this._type = type;
        }

        public override ColumnOptionBase Clone() => new GeneralColumnOption(this._type);
    }
}
