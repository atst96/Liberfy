using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Poll
    {
        [DataMember(Name = "poll")]
        public string Id { get; private set; }

        [DataMember(Name = "expires_at")]
        public string ExpiresAt { get; private set; }

        [DataMember(Name = "expired")]
        public bool Expired { get; private set; }

        [DataMember(Name = "multiple")]
        public bool Multiple { get; private set; }

        [DataMember(Name = "votes_count")]
        public int VotesCount { get; private set; }

        [DataMember(Name = "options")]
        public PollOption[] Options { get; private set; }

        [DataMember(Name = "voted")]
        public bool? Voted { get; private set; }
    }
}
