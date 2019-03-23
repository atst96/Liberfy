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
        [Utf8Json.JsonFormatter(typeof(Formatters.StringToLongFormatter))]
        public long Id { get; private set; }

        [DataMember(Name = "following")]
        public bool Following { get; private set; }

        [DataMember(Name = "followed_by")]
        public bool FollowedBy { get; private set; }

        [DataMember(Name = "blocking")]
        public bool Blocking { get; private set; }

        [DataMember(Name = "muting")]
        public bool Muting { get; private set; }

        [DataMember(Name = "muting_notifications")]
        public bool MutingNotifications { get; private set; }

        [DataMember(Name = "requested")]
        public bool Requested { get; private set; }

        [DataMember(Name = "domain_blocking")]
        public bool DomainBlocking { get; private set; }

        [DataMember(Name = "showing_reblogs")]
        public bool ShowingReblogs { get; private set; }

        [DataMember(Name = "endorsed")]
        public bool Endorsed { get; set; }
    }
}
