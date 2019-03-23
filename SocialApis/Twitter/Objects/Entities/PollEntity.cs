using System;
using System.Runtime.Serialization;
using Utf8Json;

namespace SocialApis.Twitter
{
    [DataContract]
    public class PollEntity : EntityBase
    {
        [DataMember(Name = "options")]
        public PollOption Options { get; set; }

        [DataMember(Name = "end_datetime")]
        [JsonFormatter(typeof(TwitterTimeFormatFormatter))]
        public DateTimeOffset EndDateTime { get; set; }

        [DataMember(Name = "duration_minutes")]
        public int DurationMinutes { get; set; }
    }
}
