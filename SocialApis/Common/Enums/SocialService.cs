using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    public enum SocialService : ushort
    {
        [EnumMember(Value = "twitter")]
        Twitter = 1,

        [EnumMember(Value = "mastodon")]
        Mastodon = 2,
    }
}
