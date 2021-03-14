using System;
using System.Runtime.Serialization;
using Liberfy.Data.InstanceKeys;

namespace Liberfy
{
    [DataContract]
    internal class ClientKeyCache
    {
        public ClientKeyCache(Uri uri, MastodonKeyInfo keyInfo)
        {
            this.Service = ServiceType.Mastodon;
            this.RegisteredAt = DateTimeOffset.Now;
            this.ClientId = keyInfo.ClientId;
            this.ClientSecret = keyInfo.ClientSecret;
            this.Host = uri.ToString();
        }

        [DataMember(Name = "service")]
        public ServiceType Service { get; set; }

        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "registered_at")]
        public DateTimeOffset RegisteredAt { get; set; }

        [DataMember(Name = "client_id")]
        public string ClientId { get; set; }

        [DataMember(Name = "client_secret")]
        public string ClientSecret { get; set; }
    }
}
