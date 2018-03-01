using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class EnvelopesStreamResponse : IStreamResponse
    {
        public StreamType Type { get; } = StreamType.Envelopes;

        [DataMember(Name = "for_user")]
        public long ForUser { get; private set; }

        [DataMember(Name = "messages")]
        public FriendsStreamResponse Messages { get; private set; }
    }
}
