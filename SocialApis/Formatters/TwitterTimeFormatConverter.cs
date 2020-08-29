using System;
using System.Globalization;
using SocialApis.Extensions;
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
