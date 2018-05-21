using System.Linq;
using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class EntityBase
    {
        [DataMember(Name = "indices")]
        public int[] Indices { get; set; }

        [IgnoreDataMember]
        public int IndexStart => this.Indices[0];

        [IgnoreDataMember]
        public int IndexEnd => this.Indices[1];
    }
}
