using System;
using SocialApis.Extensions;
using Utf8Json;

namespace SocialApis.Formatters
{
    public class StringToLongFormatter : IJsonFormatter<long>
    {
        long IJsonFormatter<long>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            switch (reader.GetCurrentJsonToken())
            {
                case JsonToken.Null:
                    return default;

                case JsonToken.Number:
                    return formatterResolver.ReadValue<long>(ref reader);

                case JsonToken.String:
                    var value = formatterResolver.ReadValue<string>(ref reader);
                    return long.Parse(value);

                default:
                    throw new NotSupportedException();
            }
        }

        void IJsonFormatter<long>.Serialize(ref JsonWriter writer, long value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteString(value.ToString());
            writer.WriteEndObject();
        }
    }
}
