using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Places
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "place_type")]
        public string PlaceType { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "full_name")]
        public string FullName { get; set; }

        [DataMember(Name = "country_code")]
        public string CountryCode { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "bounding_box")]
        public Coordinates<Point[][]> BoudingBox { get; set; }

        // [DataMember(Name = "attributes")]
    }
}
