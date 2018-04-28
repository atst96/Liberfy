using Liberfy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace Liberfy.JsonFormatter
{
    internal class ArrayColumnOptionFormatter : IJsonFormatter<IEnumerable<ColumnOptionBase>>
    {
        private static readonly ColumnOptionJsonFormatter _columnOptionFormatter = new ColumnOptionJsonFormatter();

        public IEnumerable<ColumnOptionBase> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var list = new LinkedList<ColumnOptionBase>();

            if (!reader.ReadIsNull() && reader.ReadIsBeginArray())
            {
                int count = 0;

                while (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                {
                    list.AddLast(_columnOptionFormatter.Deserialize(ref reader, formatterResolver));
                }
            }

            return list;
        }

        public void Serialize(ref JsonWriter writer, IEnumerable<ColumnOptionBase> value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteBeginArray();

            int itemCount = value.Count();

            if (itemCount >= 1)
            {
                _columnOptionFormatter.Serialize(ref writer, value.First(), formatterResolver);

                foreach (var item in value.Skip(1))
                {
                    writer.WriteValueSeparator();
                    _columnOptionFormatter.Serialize(ref writer, item, formatterResolver);
                }
            }

            writer.WriteEndArray();
        }
    }

    internal class FluidColumnOptionFormatter : IJsonFormatter<FluidCollection<ColumnOptionBase>>
    {
        private static readonly ArrayColumnOptionFormatter _formatter = new ArrayColumnOptionFormatter();

        public FluidCollection<ColumnOptionBase> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            return new FluidCollection<ColumnOptionBase>(_formatter.Deserialize(ref reader, formatterResolver));
        }

        public void Serialize(ref JsonWriter writer, FluidCollection<ColumnOptionBase> value, IJsonFormatterResolver formatterResolver)
        {
            _formatter.Serialize(ref writer, value, formatterResolver);
        }
    }
}
