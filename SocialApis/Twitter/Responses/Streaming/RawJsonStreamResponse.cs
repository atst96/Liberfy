using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public class RawJsonStreamResponse : IStreamResponse
    {
        public StreamType Type { get; } = StreamType.Raw;

        internal RawJsonStreamResponse(string json)
        {
            this.Json = json;
        }

        public string Json { get; }
    }
}
