using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class AttachmentMeta
    {
        [DataMember(Name = "small")]
        public Meta Small { get; private set; }

        [DataMember(Name = "original")]
        public Meta Original { get; private set; }
    }
}
