using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class DisconnectStreamResponse : IStreamResponse
    {
        public StreamType Type { get; } = StreamType.Disconnect;

        [DataMember(Name = "code")]
        public int Code { get; private set; }

        [DataMember(Name = "stream_name")]
        public string StreamName { get; private set; }

        [DataMember(Name = "reason")]
        public string Reason { get; private set; }
    }
}
