using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public static class FriendshipConnections
    {
        public const string None = "none";
        public const string Following = "following";
        public const string FollowedBy = "followed_by";
        public const string FollowRequested = "follow_requested";
        public const string Blocking = "blocking";
        public const string Muting = "muting";
    }
}
