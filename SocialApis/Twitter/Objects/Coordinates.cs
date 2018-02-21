using System.Linq;
using System.Runtime.Serialization;
using Utf8Json;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct Coordinates<T>
    {
        [DataMember(Name = "type")]
        public CoordinateType Type { get; private set; }

        [DataMember(Name = "coordinates")]
        public T Value { get; private set; }
    }
}
