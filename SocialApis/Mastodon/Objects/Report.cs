using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Report
    {
        [DataMember(Name = "id")]
        public long Id { get; private set; }

        [DataMember(Name = "action_taken")]
        public string ActionTaken { get; private set; }
    }
}
