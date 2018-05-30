using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Settings
{
    internal enum SocialService
    {
        Unknown,

        [EnumMember(Value = "twitter")]
        Twitter,

        //[EnumMember(Value = "mastodon")]
        //Mastodon,

        //[EnumMember(Value = "frost")]
        //Frost,
    }
}
