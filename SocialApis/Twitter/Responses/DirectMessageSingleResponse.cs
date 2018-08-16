using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class DirectMessageSingleResponse
    {
        [DataMember(Name = "event")]
        public DirectMessageEvent Event { get; private set; }

        [DataMember(Name = "apps")]
        public IReadOnlyDictionary<long, App> Apps { get; private set; }
    }
}
