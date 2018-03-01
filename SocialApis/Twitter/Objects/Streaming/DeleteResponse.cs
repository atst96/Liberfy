using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class DeleteResponse
    {
        [DataMember(Name = "id")]
        public long Id { get; private set; }

        [DataMember(Name = "user_id")]
        public long UserId { get; private set; }
    }
}
