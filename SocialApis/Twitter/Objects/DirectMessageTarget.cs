using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class DirectMessageTarget
    {
        [DataMember(Name = "recipient_id")]
        public long RecipientId { get; private set; }
    }
}
