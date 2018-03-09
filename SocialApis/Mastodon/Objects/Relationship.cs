using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Relationship
    {
        [DataMember(Name = "id")]
        [Utf8Json.JsonFormatter(typeof(Formatters.ToLongFormatter))]
        public long Id { get; private set; }

        [DataMember(Name = "following")]
        public bool IsFollowing { get; private set; }

        [DataMember(Name = "followed_by")]
        public bool IsFollowedBy { get; private set; }

        [DataMember(Name = "blocking")]
        public bool IsBlocking { get; private set; }

        [DataMember(Name = "muting")]
        public bool IsMuting { get; private set; }

        [DataMember(Name = "muting_notifications")]
        public bool IsMutingNotifications { get; private set; }

        [DataMember(Name = "requested")]
        public bool IsRequested { get; private set; }

        [DataMember(Name = "domain_blocking")]
        public bool IsDomainBlocking { get; private set; }
    }
}
