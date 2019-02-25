using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public enum AttachmentType
    {
        [EnumMember(Value = "image")]
        Image,
        
        [EnumMember(Value = "video")]
        Video,
        
        [EnumMember(Value = "gifv")]
        GifVideo,

        [EnumMember(Value = "unknown")]
        Unknown,

        [EnumMember(Value = "audio")]
        Audio,
    }
}
