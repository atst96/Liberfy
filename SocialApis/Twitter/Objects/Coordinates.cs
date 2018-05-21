using System.Linq;
using System.Runtime.Serialization;
using Utf8Json;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Coordinates<T>
    {
        [DataMember(Name = "type")]
        public CoordinateType Type { get; set; }

        [DataMember(Name = "coordinates")]
        public T Value { get; set; }
    }
}
