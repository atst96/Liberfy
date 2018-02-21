using System;
using System.Runtime.Serialization;
using Utf8Json;

namespace SocialApis.Twitter
{
    [DataContract]
    public class PollEntity : EntityBase
    {
        [DataMember(Name = "options")]
        public PollOption Options { get; private set; }

        [DataMember(Name = "end_datetime")]
        [JsonFormatter(typeof(DateTimeOffsetFormatter))]
        public DateTimeOffset EndDateTime { get; private set; }

        [DataMember(Name = "duration_minutes")]
        public int DurationMinutes { get; private set; }
    }
}
