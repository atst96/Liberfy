using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UrlEntity : EntityBase
    {
        [DataMember(Name = "url")]
        private string _url;
        public string Url => _url;

        [DataMember(Name = "display_url")]
        private string _displayUrl;
        public string DisplayUrl => _displayUrl;

        [DataMember(Name = "expanded_url")]
        private string _expandedUrl;
        public string ExpandedUrl => _expandedUrl;

        [DataMember(Name = "unwound")]
        private UrlUnwound _unwound;
        public UrlUnwound Unwound => _unwound;
    }
}
