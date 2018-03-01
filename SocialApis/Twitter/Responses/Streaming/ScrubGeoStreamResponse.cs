using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public sealed class ScrubGeoStreamResponse : IStreamResponse
    {
        public StreamType Type { get; } = StreamType.ScrubGeo;

        [DataMember(Name = "user_id")]
        public long UserId { get; private set; }

        [DataMember(Name = "up_to_status_id")]
        public long UpToStatusId { get; private set; }
    }
}
