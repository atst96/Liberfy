using System.Runtime.Serialization;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class ClientKeyInfo
    {
        [DataMember(Name = "id")]
        public string Id { get; private set; }

        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "redirect_uri")]
        public string RedirectUri { get; private set; }

        [DataMember(Name = "client_id")]
        public string ClientId { get; private set; }

        [DataMember(Name = "client_secret")]
        public string ClientSecret { get; private set; }
    }
}
