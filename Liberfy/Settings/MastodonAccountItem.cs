using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SocialApis.Mastodon;

namespace Liberfy.Settings
{
    [DataContract]
    internal class MastodonAccountItem : AccountSettingBase
    {
        [DataMember(Name = "service")]
        public override ServiceType Service { get; } = ServiceType.Mastodon;

        [DataMember(Name = "account.id")]
        public long Id { get; set; }

        [DataMember(Name = "instance.url")]
        public Uri InstanceUrl { get; set; }

        [DataMember(Name = "account.username")]
        public string UserName { get; set; }

        [DataMember(Name = "account.display_name")]
        public string DisplayName { get; set; }

        [DataMember(Name = "account.avatar")]
        public string Avatar { get; set; }

        [DataMember(Name = "account.locked")]
        public bool Locked { get; set; }

        [DataMember(Name = "keys.client_id")]
        public string ClientId { get; set; }

        [DataMember(Name = "keys.client_secret")]
        public string ClientSecret { get; set; }

        [DataMember(Name = "keys.access_token")]
        public string AccessToken { get; set; }

        public MastodonApi CreateApi()
        {
            return new MastodonApi(this.InstanceUrl, this.ClientId, this.ClientSecret, this.AccessToken);
        }
    }
}
