using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis
{
    public class TwitterTimeFormatFormatter : IJsonFormatter<DateTimeOffset>
    {
        private const string TimeFormat = "ddd MMM dd HH:mm:ss +ffff yyyy";

        public DateTimeOffset Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var value = formatterResolver.ReadValue<string>(ref reader);

            return DateTimeOffset.ParseExact(value, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }

        public void Serialize(ref JsonWriter writer, DateTimeOffset value, IJsonFormatterResolver formatterResolver)
        {
            var writeValue = value.ToString(TimeFormat);

            formatterResolver.WriteValue(ref writer, writeValue);
        }
    }
}
