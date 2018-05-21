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

        [DataMember(Name = "connections")]
        public FriendshipConnection[] Connections { get; set; }

        public FriendshipConnection GetFlaggedConnections()
        {
            var connection = default(FriendshipConnection);

            for (int i = 0; i < this.Connections.Length; ++i)
            {
                connection |= this.Connections[i];
            }

            return connection;
        }
    }
}
