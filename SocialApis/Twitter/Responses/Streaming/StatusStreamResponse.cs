using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class StatusStreamResponse : Status, IStreamResponse
    {
        [IgnoreDataMember]
        public StreamType Type { get; } = StreamType.Status;
    }
}
