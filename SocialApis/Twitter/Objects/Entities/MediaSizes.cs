using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct MediaSizes
    {
        [DataMember(Name = "thumb")]
        private MediaSize _thumb;
        public MediaSize Thumb => _thumb;

        [DataMember(Name = "large")]
        private MediaSize _large;
        public MediaSize Large => _large;

        [DataMember(Name = "medium")]
        private MediaSize _medium;
        public MediaSize Medium => _medium;

        [DataMember(Name = "small")]
        private MediaSize _small;
        public MediaSize Small => _small;
    }
}
