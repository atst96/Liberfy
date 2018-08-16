using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Instance
    {
        [DataMember(Name = "uri")]
        public string Uri { get; private set; }

        [DataMember(Name = "title")]
        public string Title { get; private set; }

        [DataMember(Name = "description")]
        public string Description { get; private set; }

        [DataMember(Name = "email")]
        public string Email { get; private set; }

        [DataMember(Name = "version")]
        public string Version { get; private set; }

        [DataMember(Name = "urls")]
        public InstanceUrls Urls { get; private set; }

        [DataMember(Name = "languages")]
        public string[] Language { get; private set; }

        [DataMember(Name = "contact_account")]
        public Account ContactAccount { get; private set; }
    }
}
