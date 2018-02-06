using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class StatusResponse : Status, IRateLimit
    {
        [IgnoreDataMember]
        public RateLimit RateLimit { get; }
    }
}
