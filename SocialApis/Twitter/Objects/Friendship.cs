using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Friendship
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        /// <summary>
        /// SocialApis.Twitter.FriendshipConnections.*
        /// </summary>
        [DataMember(Name = "connections")]
        public string[] Connections { get; set; }
    }
}
