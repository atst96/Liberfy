using System.Linq;
using System.Runtime.Serialization;
using Utf8Json;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Coordinates
    {
        [SerializationConstructor]
        private Coordinates() { }

        internal Coordinates(long longitude, long latitude)
        {
            _coodinates = new long[2] { longitude, latitude };
        }

        [DataMember(Name = "coordinates")]
        private long[] _coodinates;

        //[DataMember(Name = "type")]
        //private string _type { get; }

        [IgnoreDataMember]
        public long? Longitude => _coodinates?.ElementAt(0);

        [IgnoreDataMember]
        public long? Latitude => _coodinates?.ElementAt(1);
    }
}
