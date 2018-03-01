using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public sealed class UserWithheldStreamResponse : IStreamResponse
    {
        public StreamType Type { get; } = StreamType.UserWithheld;

        [DataMember(Name = "id")]
        public long Id { get; private set; }

        [DataMember(Name = "withheld_in_countries")]
        public string[] WithheldInCountries { get; private set; }
    }
}
