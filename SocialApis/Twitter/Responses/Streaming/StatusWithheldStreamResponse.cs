using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public sealed class StatusWithheldStreamResponse : IStreamResponse
    {
        public StreamType Type { get; } = StreamType.StatusWithheld;

        [DataMember(Name = "id")]
        public long Id { get; private set; }

        [DataMember(Name = "user_id")]
        public long UserId { get; private set; }

        [DataMember(Name = "withheld_in_countries")]
        public string[] WithheldInCountries { get; private set; }
    }
}
