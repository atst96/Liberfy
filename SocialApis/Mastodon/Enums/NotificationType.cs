using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public enum NotificationType
    {
        [EnumMember(Value = "mention")]
        Mention,

        [EnumMember(Value = "reblog")]
        Reblog,

        [EnumMember(Value = "favourite")]
        Favourite,

        [EnumMember(Value = "follow")]
        Follow,

        [EnumMember(Value = "poll")]
        Poll,
    }
}
