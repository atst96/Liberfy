using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Twitter
{
    [DataContract]
    public sealed class EventStreamResponse : IStreamResponse
    {
        public StreamType Type { get; } = StreamType.Event;

        [DataMember(Name = "created_at")]
        [JsonFormatter(typeof(DateTimeOffsetFormatter))]
        public DateTimeOffset CreatedAt { get; private set; }

        [DataMember(Name = "event")]
        public EventType EventType { get; private set; }

        [DataMember(Name = "source")]
        public User Source { get; private set; }

        [DataMember(Name = "target")]
        public User Target { get; private set; }

        [IgnoreDataMember]
        public Status TargetStauts { get; internal set; }

        [IgnoreDataMember]
        public List TargetList { get; internal set; }

        [IgnoreDataMember]
        public object TargetApplication { get; internal set; }
    }
}
