using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Formatters
{
    public class NullableValueFormatter<T> : IJsonFormatter<T?> where T : struct
    {
        T? IJsonFormatter<T?>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.GetCurrentJsonToken() == JsonToken.Null)
            {
                return default;
            }

            var formatter = GetFormatter(formatterResolver);

            return formatter.Deserialize(ref reader, formatterResolver);
        }

        void IJsonFormatter<T?>.Serialize(ref JsonWriter writer, T? value, IJsonFormatterResolver formatterResolver)
        {
            if (!value.HasValue)
            {
                writer.WriteNull();
                writer.WriteEndObject();
                return;
            }

            GetFormatter(formatterResolver)
                .Serialize(ref writer, value.Value, formatterResolver);
        }

        private static IJsonFormatter<T> GetFormatter(IJsonFormatterResolver formatterResolver)
        {
            var valueType = typeof(T);

            if (valueType == typeof(TimeSpan))
            {
                return (IJsonFormatter<T>)new TimeSpanFormatter();
            }

            return formatterResolver.GetFormatter<T>();
        }
    }
}
