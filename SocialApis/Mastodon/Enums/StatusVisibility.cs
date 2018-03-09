using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public enum StatusVisibility
    {
        [EnumMember(Value = "public")]
        Public,

        [EnumMember(Value = "unlisted")]
        Unlisted,

        [EnumMember(Value = "private")]
        Private,
        
        [EnumMember(Value = "direct")]
        Direct,
    }
}
