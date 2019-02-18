using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal enum ServiceType : ushort
    {
        [EnumMember(Value = "twitter")]
        Twitter = 1,

        [EnumMember(Value = "mastodon")]
        Mastodon,
    }
}
