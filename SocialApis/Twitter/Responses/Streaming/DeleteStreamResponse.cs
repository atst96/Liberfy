using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public sealed class DeleteStreamResponse : IStreamResponse
    {
        public StreamType Type { get; } = StreamType.Delete;

        [DataMember(Name = "status")]
        public DeleteResponse Status { get; private set; }

        [DataMember(Name = "direct_message")]
        public DeleteResponse DirectMessage { get; private set; }
    }
}
