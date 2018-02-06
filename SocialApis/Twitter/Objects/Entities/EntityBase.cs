using System.Linq;
using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class EntityBase
    {
        [DataMember(Name = "indices")]
        protected long[] _indices { get; }

        [IgnoreDataMember]
        public long IndexStart => _indices?.ElementAt(0) ?? 0;

        [IgnoreDataMember]
        public long IndexEnd => _indices?.ElementAt(1) ?? 0;
    }
}
