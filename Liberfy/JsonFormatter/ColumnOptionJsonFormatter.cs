using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace Liberfy.JsonFormatter
{
    internal class ColumnOptionJsonFormatter : IJsonFormatter<ColumnOptionBase>
    {
        static readonly Dictionary<string, ColumnType> _columnTypeParis;

        static ColumnOptionJsonFormatter()
        {
            _columnTypeParis = new Dictionary<string, ColumnType>();

            var enumType = typeof(ColumnType);

            foreach (var memberInfo in enumType.GetMembers(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            {
                var enumAttr = memberInfo
                    .GetCustomAttributes(false)
                    .OfType<EnumMemberAttribute>()
                    .FirstOrDefault();

                var columnType = (ColumnType)Enum.Parse(enumType, memberInfo.Name);

                if (enumAttr != null)
                    _columnTypeParis.Add(enumAttr.Value, columnType);

                _columnTypeParis.Add(memberInfo.Name, columnType);
            }
        }

        static T Deserialize<T>(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            try
            {
                return formatterResolver.GetFormatter<T>().Deserialize(ref reader, formatterResolver);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ColumnOptionBase Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            // 現在のJsonReaderのオフセット（`{`の直前）
            int objectOffset = reader.GetCurrentOffsetUnsafe();

            if (reader.ReadIsNull())
            {
                return null;
            }
            else if (reader.ReadIsBeginObject())
            {
                int count = 0;

                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
                {
                    if (reader.ReadPropertyName() == "type")
                    {
                        var memberName = reader.ReadString();

                        if (!_columnTypeParis.TryGetValue(memberName, out var type))
                            throw new NotSupportedException();

                        // JsonReaderのオフセットを直前のオブジェクト開始位置（`{`）まで戻してデシリアライズ
                        reader.AdvanceOffset(objectOffset - reader.GetCurrentOffsetUnsafe());

                        switch (type)
                        {
                            case ColumnType.List:
                                return Deserialize<ListColumnOption>(ref reader, formatterResolver);

                            case ColumnType.Search:
                                return Deserialize<SearchColumnOption>(ref reader, formatterResolver);

                            default:
                                return Deserialize<GeneralColumnOption>(ref reader, formatterResolver);
                        }
                    }
                }

                return null;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private static IJsonFormatter<object> _objectJsonFormatter;

        public void Serialize(ref JsonWriter writer, ColumnOptionBase value, IJsonFormatterResolver formatterResolver)
        {
            if (_objectJsonFormatter == null)
                _objectJsonFormatter = formatterResolver.GetFormatter<object>();

            _objectJsonFormatter.Serialize(ref writer, value, formatterResolver);
        }
    }
}
