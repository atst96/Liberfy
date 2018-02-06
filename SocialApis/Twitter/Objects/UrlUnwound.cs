using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UrlUnwound
    {
        [DataMember(Name = "url")]
        private string _url;
        public string Url => _url;

        [DataMember(Name = "status")]
        private int _status;
        public int Status => _status;

        [DataMember(Name = "title")]
        private string _title;
        public string Title => _title;

        [DataMember(Name = "description")]
        private string _description;
        public string Description => _description;
    }
}
