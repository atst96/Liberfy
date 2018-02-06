using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class ListedResponse<T> : Collection<T>, IRateLimit
    {
        [IgnoreDataMember]
        public RateLimit RateLimit { get; }
    }
}
