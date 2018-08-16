using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class InstanceUrls
    {
        [DataMember(Name = "streaming_api")]
        public string StreamingApi { get; private set; }
    }
}
