using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class TimelineResponse
    {
        [DataMember(Name = "response")]
        public CollectionResponseInfo Response { get; private set; }

        [DataMember(Name = "objects")]
        public CollectionObjects Objects { get; private set; }
    }
}
