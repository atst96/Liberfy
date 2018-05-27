using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct VideoVariant
    {
        [DataMember(Name = "bitrate")]
        public int Bitrate { get; set; }

        [DataMember(Name = "content_type")]
        public string ContentType { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
