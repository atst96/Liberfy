using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public enum FriendshipConnection : int
    {
        [EnumMember(Value = "none")]
        None = 0b0_0000,

        [EnumMember(Value = "following")]
        Following = 0b0_0001,

        [EnumMember(Value = "followed_by")]
        FollowedBy = 0b0_0010,

        [EnumMember(Value = "follow_requested")]
        FollowRequested = 0b0_0100,

        [EnumMember(Value = "blocking")]
        Blocking = 0b0_1000,

        [EnumMember(Value = "muting")]
        Muting = 0b1_0000,
    }
}
