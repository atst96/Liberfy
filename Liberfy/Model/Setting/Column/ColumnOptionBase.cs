using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Liberfy
{
    [DataContract, Serializable]
    internal abstract class ColumnOptionBase : NotificationObject, ICloneable
    {
        [DataMember(Name = "type")]
        public abstract ColumnType Type { get; }

        public static Type GetOptionClassType(ColumnType type)
        {
            switch (type)
            {
                case ColumnType.List:
                    return typeof(ListColumnOption);

                case ColumnType.Search:
                    return typeof(SearchColumnOption);

                default:
                    return typeof(GeneralColumnOption);
            }
        }

        public static ColumnOptionBase GetDefault(ColumnType type)
        {
            switch (type)
            {
                case ColumnType.List:
                    return new ListColumnOption();

                case ColumnType.Search:
                    return new SearchColumnOption();

                default:
                    return new GeneralColumnOption(type);
            }
        }

        internal static ColumnOptionBase CreateFromDefault(ColumnOptionBase columnOption)
        {
            return columnOption.Clone();
        }

        public virtual ColumnOptionBase Clone()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);

                ms.Position = 0;

                return (ColumnOptionBase)formatter.Deserialize(ms);
            }
        }

        object ICloneable.Clone() => this.Clone();
    }
}
