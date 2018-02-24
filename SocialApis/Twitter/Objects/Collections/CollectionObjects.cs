using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class CollectionObjects
    {
        [DataMember(Name = "users")]
        public IReadOnlyDictionary<long, User> Users { get; private set; }

        [DataMember(Name = "timelines")]
        public IReadOnlyDictionary<string, Timeline> Timelines { get; private set; }
    }
}
