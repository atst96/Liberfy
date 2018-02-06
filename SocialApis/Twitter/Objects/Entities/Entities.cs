using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct Entities
    {
        [DataMember(Name = "hashtags")]
        private HashtagEntity[] _hashTags;
        public HashtagEntity[] Hashtags => _hashTags;

        [DataMember(Name = "urls")]
        private UrlEntity[] _urls;
        public UrlEntity[] Urls => _urls;

        [DataMember(Name = "user_mentions")]
        private UserMentionEntity[] _userMentions;
        public UserMentionEntity[] UserMentions => _userMentions;

        [DataMember(Name = "symbols")]
        private HashtagEntity[] _symbols;
        public HashtagEntity[] Symbols => _symbols;

        [DataMember(Name = "media")]
        private MediaEntity[] _media;
        public MediaEntity[] Media => _media;
    }
}
