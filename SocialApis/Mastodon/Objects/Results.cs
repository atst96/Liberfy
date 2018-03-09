using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Results
    {
        [DataMember(Name = "accounts")]
        public Account[] Accounts { get; private set; }

        [DataMember(Name = "statuses")]
        public Status[] Statuses { get; private set; }

        [DataMember(Name = "hashtags")]
        public string[] Hashtags { get; private set; }
    }
}
