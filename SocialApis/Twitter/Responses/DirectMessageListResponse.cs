using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Responses
{
    [DataContract]
    public class DirectMessageListResponse
    {
        [DataMember(Name = "events")]
        public DirectMessageEvent[] Events { get; private set; }

        [DataMember(Name = "apps")]
        public IReadOnlyDictionary<long, App> Apps { get; private set; }

        [DataMember(Name = "next_cursor")]
        public long NextCursor { get; private set; }

        [DataMember(Name = "previous_cursor")]
        public long PreviousCursor { get; private set; }
    }
}
