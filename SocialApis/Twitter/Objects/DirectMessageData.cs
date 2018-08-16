using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class DirectMessageData
    {
        [DataMember(Name = "text")]
        public string Text { get; private set; }

        [DataMember(Name = "entities")]
        public Entities Entities { get; private set; }
    }
}
