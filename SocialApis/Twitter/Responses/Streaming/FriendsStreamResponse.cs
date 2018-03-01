using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class FriendsStreamResponse : IStreamResponse
    {
        public StreamType Type { get; } = StreamType.Friends;

        [DataMember(Name = "friends")]
        public long[] Friends { get; set; }
    }
}
