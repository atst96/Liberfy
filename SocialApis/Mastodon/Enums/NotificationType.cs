using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public static class NotificationTypes
    {
        public const string Mention = "mention";
        public const string Reblog = "reblog";
        public const string Favourite = "favourite";
        public const string Follow = "follow";
        public const string Poll = "poll";
    }
}
