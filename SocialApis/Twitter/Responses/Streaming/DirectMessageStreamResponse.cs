using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class DirectMessageStreamResponse : DirectMessage, IStreamResponse
    {
        public StreamType Type { get; } = StreamType.DirectMessage;
    }
}
