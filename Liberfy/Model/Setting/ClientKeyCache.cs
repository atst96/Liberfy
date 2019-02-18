using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    [DataContract]
    internal class ClientKeyCache
    {
        public ClientKeyCache(Uri uri, SocialApis.Mastodon.ClientKeyInfo keyInfo)
        {
            this.Service = ServiceType.Mastodon;
            this.Id = keyInfo.Id;
            this.RegisteredAt = DateTimeOffset.Now;
            this.ClientId = keyInfo.ClientId;
            this.ClientSecret = keyInfo.ClientSecret;
            this.Host = uri.ToString();
        }

        [Key("service")]
        [DataMember(Name = "service")]
        public ServiceType Service { get;  set; }

        [Key("id")]
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [Key("host")]
        [DataMember(Name = "host")]
        public string Host { get; set; }

        [Key("registered_at")]
        [DataMember(Name = "registered_at")]
        public DateTimeOffset RegisteredAt { get; set; }

        [Key("client_id")]
        [DataMember(Name = "client_id")]
        public string ClientId { get; set; }

        [Key("client_secret")]
        [DataMember(Name = "client_secret")]
        public string ClientSecret { get; set; }
    }
}
