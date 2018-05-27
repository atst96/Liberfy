using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public enum MediaType
    {
        [EnumMember(Value = "photo")]
        Photo,

        [EnumMember(Value = "animated_gif")]
        AnimatedGif,

        [EnumMember(Value = "video")]
        Video,
    }
}
