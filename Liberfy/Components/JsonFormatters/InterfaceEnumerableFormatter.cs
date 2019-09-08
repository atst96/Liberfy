using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace Liberfy.Components.JsonFormatters
{
    internal abstract class UnionInterfaceEnumerableFormatterBase<T> : IJsonFormatter<IEnumerable<T>>
    {
        public IEnumerable<T> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            var items = new LinkedList<T>();

            int count = 0;
            while (reader.ReadIsInArray(ref count))
            {
                items.AddLast(this.DeserializeItem(ref reader, formatterResolver));
            }

            return items.ToArray();
        }

        public void Serialize(ref JsonWriter writer, IEnumerable<T> value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteBeginArray();

            int count = value.Count();
            int idx = 0;

            foreach (T item in value)
            {
                this.SerializeItem(ref writer, item, formatterResolver);

                if (++idx < count)
                {
                    writer.WriteValueSeparator();
                }
            }

            writer.WriteEndArray();
        }

        protected abstract T DeserializeItem(ref JsonReader reader, IJsonFormatterResolver formatterResolver);

        protected abstract void SerializeItem(ref JsonWriter writer, T value, IJsonFormatterResolver formatterResolver);
    }
}
