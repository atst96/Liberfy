using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Formatters
{
    public class TimeSpanFormatter : IJsonFormatter<TimeSpan>
    {
        TimeSpan IJsonFormatter<TimeSpan>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.GetCurrentJsonToken() != JsonToken.String)
            {
                throw new NotSupportedException();
            }

            var value = formatterResolver.ReadValue<string>(ref reader);

            return TimeSpan.Parse(value);
        }

        void IJsonFormatter<TimeSpan>.Serialize(ref JsonWriter writer, TimeSpan value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteString(value.ToString());
            writer.WriteEndObject();
        }
    }
}
