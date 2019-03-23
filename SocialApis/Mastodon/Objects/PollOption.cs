using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class PollOption
    {
        [DataMember(Name = "title")]
        public string Title { get; private set; }

        [DataMember(Name = "votes_count")]
        public int? VotesCount { get; private set; }
    }
}
