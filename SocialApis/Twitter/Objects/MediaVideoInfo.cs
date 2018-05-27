using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct MediaVideoInfo
    {
        [DataMember(Name = "aspect_ratio")]
        public int[] AspectRatio { get; set; }

        [DataMember(Name = "variants")]
        public VideoVariant[] Variants { get; set; }
    }
}
