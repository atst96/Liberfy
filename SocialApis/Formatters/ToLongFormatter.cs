using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Formatters
{
    public class ToLongFormatter : Utf8Json.IJsonFormatter<long>
    {
        long IJsonFormatter<long>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            switch (reader.GetCurrentJsonToken())
            {
                case JsonToken.Null:
                    return default(long);

                case JsonToken.Number:
                    return reader.ReadInt64();

                case JsonToken.String:
                    return long.Parse(reader.ReadString());

                default:
                    throw new NotSupportedException();
            }
        }

        void IJsonFormatter<long>.Serialize(ref JsonWriter writer, long value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteString(value.ToString());
        }
    }
}
