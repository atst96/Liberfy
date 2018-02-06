using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis
{
    public class DateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset>
    {
        private const string TwitterTimeFormat = "ddd MMM dd HH:mm:ss +ffff yyyy";

        public DateTimeOffset Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var value = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, formatterResolver);
            return DateTimeOffset.ParseExact(
                value,
                TwitterTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal);
        }

        public void Serialize(ref JsonWriter writer, DateTimeOffset value, IJsonFormatterResolver formatterResolver)
        {
            throw new NotImplementedException();
            //writer.WriteString(value.ToString(TwitterTimeFormat));
        }
    }
}
