using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SocialApis.Twitter;

namespace Liberfy.Settings
{
    [DataContract]
    internal class TwitterAccountItem : AccountSettingBase
    {
        [DataMember(Name = "service", Order = 0)]
        public override ServiceType Service { get; } = ServiceType.Twitter;

        [DataMember(Name = "user.id")]
        public long Id { get; set; }

        [DataMember(Name = "user.screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "user.name")]
        public string Name { get; set; }

        [DataMember(Name = "user.is_protected")]
        public bool IsProtected { get; set; }

        [DataMember(Name = "user.profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [DataMember(Name = "keys.consumer_key")]
        public string ConsumerKey { get; set; }

        [DataMember(Name = "keys.consumer_secret")]
        public string ConsumerSecret { get; set; }

        [DataMember(Name = "keys.access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "keys.access_token_secret")]
        public string AccessTokenSecret { get; set; }

        public TwitterApi CreateApi()
        {
            return new TwitterApi(this.ConsumerKey, this.ConsumerSecret, this.AccessToken, this.AccessTokenSecret);
        }
    }
}
