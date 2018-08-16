using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class DirectMessageCreate
    {
        [DataMember(Name = "target")]
        public DirectMessageTarget Target { get; private set; }

        [DataMember(Name = "sender_id")]
        public long SenderId { get; private set; }

        [DataMember(Name = "source_app_id")]
        public long SourceAppId { get; private set; }

        [DataMember(Name = "message_data")]
        public DirectMessageData MessageData { get; private set; }
    }
}
