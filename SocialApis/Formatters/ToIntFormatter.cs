using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Formatters
{
    public class ToIntFormatter : IJsonFormatter<int>
    {
        int IJsonFormatter<int>.Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            switch (reader.GetCurrentJsonToken())
            {
                case JsonToken.Null:
                    return default(int);

                case JsonToken.Number:
                    return reader.ReadInt32();

                case JsonToken.String:
                    return int.Parse(reader.ReadString());

                default:
                    throw new NotSupportedException();
            }
        }

        void IJsonFormatter<int>.Serialize(ref JsonWriter writer, int value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteString(value.ToString());
        }
    }
}
