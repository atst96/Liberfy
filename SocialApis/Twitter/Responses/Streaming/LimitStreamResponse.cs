using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public sealed class LimitStreamResponse : IStreamResponse
    {
        public StreamType Type { get; } = StreamType.Limit;

        [DataMember(Name = "track")]
        public long Track { get; private set; }
    }
}
