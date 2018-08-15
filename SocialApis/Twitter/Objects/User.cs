using SocialApis.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Twitter
{
    [DataContract]
    public class User : IAccount
    {
        [IgnoreDataMember]
        public SocialService Service { get; } = SocialService.Twitter;

        [DataMember(Name = "id")]
        public long? Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "entities")]
        public UserEntities Entities { get; set; }

        // [DataMember(Name = "derived")]

        [DataMember(Name = "protected")]
        public bool IsProtected { get; set; }

        [DataMember(Name = "verified")]
        public bool IsVerified { get; set; }

        [DataMember(Name = "followers_count")]
        public int FollowersCount { get; set; }

        [DataMember(Name = "friends_count")]
        public int FriendsCount { get; set; }

        [DataMember(Name = "listed_count")]
        public int? ListedCount { get; set; }

        [DataMember(Name = "favourites_count")]
        public int FavoritesCount { get; set; }

        [DataMember(Name = "statuses_count")]
        public int StatusesCount { get; set; }

        [DataMember(Name = "created_at")]
        [JsonFormatter(typeof(DateTimeOffsetFormatter))]
        public DateTimeOffset CreatedAt { get; set; }

        [DataMember(Name = "utc_offset")]
        public int? UtcOffset { get; set; }

        [DataMember(Name = "time_zone")]
        public string TimeZone { get; set; }

        [DataMember(Name = "geo_enabled")]
        public bool IsGeoEnabled { get; set; }

        [DataMember(Name = "lang")]
        public string Language { get; set; }

        [DataMember(Name = "contributors_enabled")]
        public bool IsContributorsEnabled { get; set; }

        [DataMember(Name = "profile_background_color")]
        public string ProfileBackgroundColor { get; set; }

        [DataMember(Name = "profile_background_image_url")]
        public string ProfileBackgroundImageUrl { get; set; }

        [DataMember(Name = "profile_background_image_url_https")]
        public string ProfileBackgroundImageUrlHttps { get; set; }

        [DataMember(Name = "profile_background_tile")]
        public bool ProfileBackgroundTile { get; set; }

        [DataMember(Name = "profile_banner_url")]
        public string ProfileBannerUrl { get; set; }

        [DataMember(Name = "profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [DataMember(Name = "profile_image_url_https")]
        public string ProfileImageUrlHttps { get; set; }

        [DataMember(Name = "profile_link_color")]
        public string ProfileLinkColor { get; set; }

        [DataMember(Name = "profile_sidebar_border_color")]
        public string ProfileSidebarBorderColor { get; set; }

        [DataMember(Name = "profile_sidebar_fill_color")]
        public string ProfileSidebarFillColor { get; set; }

        [DataMember(Name = "profile_text_color")]
        public string ProfileTextColor { get; set; }

        [DataMember(Name = "profile_use_background_Image")]
        public bool IsProfileUseBackgroundImage { get; set; }

        [DataMember(Name = "default_profile")]
        public bool IsDefaultProfile { get; set; }

        [DataMember(Name = "default_profile_image")]
        public bool IsDefaultProfileImage { get; set; }

        [DataMember(Name = "withheld_in_countries")]
        public string WithheldInCountries { get; set; }

        [DataMember(Name = "withheld_scope")]
        public string WithheldScope { get; set; }

        [DataMember(Name = "is_translation_enabled")]
        public bool IsTranslationEnabled { get; set; }

        [DataMember(Name = "profile_location")]
        public Places ProfileLocation { get; set; }

        [DataMember(Name = "suspended")]
        public bool? IsSuspended { get; set; }

        [DataMember(Name = "needs_phone_verification")]
        public bool? NeedsPhoneVerification { get; set; }

        [DataMember(Name = "translator_type")]
        public string TranslatorType { get; set; }
    }
}
