using System.Linq;
using System.Runtime.Serialization;
using Utf8Json;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Coordinates<T>
    {
        /// <summary>
        /// SocialApis.Twitter.CoordinateTypes.*
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "coordinates")]
        public T Value { get; set; }
    }
}
