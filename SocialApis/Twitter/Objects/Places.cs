using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Places
    {
        [DataMember(Name = "id")]
        public string Id { get; private set; }

        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "place_type")]
        public string PlaceType { get; private set; }

        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "full_name")]
        public string FullName { get; private set; }

        [DataMember(Name = "country_code")]
        public string CountryCode { get; private set; }

        [DataMember(Name = "country")]
        public string Country { get; private set; }

        [DataMember(Name = "bounding_box")]
        public Coordinates<Point[][]> BoudingBox { get; private set; }

        // [DataMember(Name = "attributes")]
    }
}
