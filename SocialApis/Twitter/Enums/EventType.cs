using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public enum EventType : int
    {
        Unknown = 0x0000,
        Tweet   = 0x0010,
        List    = 0x0020,
        User    = 0x0030,
        App     = 0x0040,

        [EnumMember(Value = "access_revoked")]
        AccessRevoked = 0x0001 | App,

        [EnumMember(Value = "block")]
        Block = 0x0001 | User,

        [EnumMember(Value = "unblock")]
        Unblock = 0x0002 | User,

        [EnumMember(Value = "follow")]
        Follow = 0x0003 | User,

        [EnumMember(Value = "unfollow")]
        Unfollow = 0x0004 | User,

        [EnumMember(Value = "list_created")]
        ListCreated = 0x0001 | List,

        [EnumMember(Value = "list_destroyed")]
        ListDestroyed = 0x0002 | List,

        [EnumMember(Value = "list_updated")]
        ListUpdated = 0x0005 | List,

        [EnumMember(Value = "list_member_added")]
        ListMemberAdded = 0x0004 | List,

        [EnumMember(Value = "list_member_removed")]
        ListMemberRemoved = 0x0005 | List,

        [EnumMember(Value = "list_user_subscribed")]
        ListUserSubscribed = 0x0006 | List,

        [EnumMember(Value = "list_user_unsubscribed")]
        ListUserUnsubscribed = 0x0007 | List,

        [EnumMember(Value = "favorite")]
        Favorite = 0x0001 | Tweet,

        [EnumMember(Value = "unfavorite")]
        Unfavorite = 0x0002 | Tweet,

        [EnumMember(Value = "quoted_tweet")]
        QuotedTweet = 0x0003 | Tweet,

        [EnumMember(Value = "user_update")]
        UserUpdate = 0x0007 | User,
    }
}
