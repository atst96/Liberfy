using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Formatters
{
    public class StringToIntFormatter : IJsonFormatter<int>
    {
        int IJsonFormatter<int>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            switch (reader.GetCurrentJsonToken())
            {
                case JsonToken.Null:
                    return default;

                case JsonToken.Number:
                    return formatterResolver.ReadValue<int>(ref reader);

                case JsonToken.String:
                    var value = formatterResolver.ReadValue<string>(ref reader);
                    return int.Parse(value);

                default:
                    throw new NotSupportedException();
            }
        }

        void IJsonFormatter<int>.Serialize(ref JsonWriter writer, int value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteString(value.ToString());
            writer.WriteEndObject();
        }
    }
}
