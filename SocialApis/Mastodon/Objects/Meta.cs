using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public struct Meta
    {
        [DataMember(Name = "width")]
        public int Width { get; private set; }

        [DataMember(Name = "height")]
        public int Height { get; private set; }

        [DataMember(Name = "size")]
        public string Size { get; private set; }

        [DataMember(Name = "aspect")]
        public float Aspect { get; private set; }

        [DataMember(Name = "frame_rate")]
        public string FrameRate { get; private set; }

        [DataMember(Name = "duration")]
        public double Duration { get; set; }

        [DataMember(Name = "bitrate")]
        public int Bitrate { get; private set; }
    }
}
