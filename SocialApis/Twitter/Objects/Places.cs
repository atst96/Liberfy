using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Places
    {
        [DataMember(Name = "id")]
        private string _id;
        public string Id => _id;

        [DataMember(Name = "url")]
        private string _url;
        public string Url => _url;

        [DataMember(Name = "place_type")]
        private string _placeType;
        public string PlaceType => _placeType;

        [DataMember(Name = "name")]
        private string _name;
        public string Name => _name;

        [DataMember(Name = "full_name")]
        private string _fullName;
        public string FullName => _fullName;

        [DataMember(Name = "country_code")]
        private string _countryCode;
        public string CountryCode => _countryCode;

        [DataMember(Name = "country")]
        private string _country;
        public string Country => _country;

        // [DataMember(Name = "bounding_box")]

        // [DataMember(Name = "attributes")]
    }
}
