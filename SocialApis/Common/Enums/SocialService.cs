using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    public enum SocialService
    {
        [EnumMember(Value = "twitter")]
        Twitter,

        [EnumMember(Value = "mastodon")]
        Mastodon,
    }
}
