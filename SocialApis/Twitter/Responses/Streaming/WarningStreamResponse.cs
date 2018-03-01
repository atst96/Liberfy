using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class WarningStreamResponse : IStreamResponse
    {
        public StreamType Type { get; } = StreamType.Warning;

        [DataMember(Name = "code")]
        public string Code { get; private set; }

        [DataMember(Name = "message")]
        public string Message { get; private set; }

        [DataMember(Name = "percent_full")]
        public int PercentFull { get; private set; }
    }
}
