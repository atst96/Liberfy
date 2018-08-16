using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class DirectMessageEvent
    {
        [DataMember(Name = "type")]
        public DirectMessageEventType Type { get; private set; }

        [DataMember(Name = "id")]
        public long Id { get; private set; }

        [DataMember(Name = "created_timestamp")]
        public DateTimeOffset CreatedTimestamp { get; private set; }

        [DataMember(Name = "message_create")]
        public DirectMessageCreate Create { get; private set; }
    }
}
