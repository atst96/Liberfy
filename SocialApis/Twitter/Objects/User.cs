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
    public class User
    {
        [DataMember(Name = "id")]
        public long Id { get; private set; }

        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; private set; }

        [DataMember(Name = "location")]
        public string Location { get; private set; }

        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "description")]
        public string Description { get; private set; }

        [DataMember(Name = "entities")]
        public UserEntities Entities { get; private set; }

        // [DataMember(Name = "derived")]

        [DataMember(Name = "protected")]
        public bool IsProtected { get; private set; }

        [DataMember(Name = "verified")]
        public bool IsVerified { get; private set; }

        [DataMember(Name = "followers_count")]
        public int FollowersCount { get; private set; }

        [DataMember(Name = "friends_count")]
        public int FriendsCount { get; private set; }

        [DataMember(Name = "listed_count")]
        public int ListedCount { get; private set; }

        [DataMember(Name = "favourites_count")]
        public int FavoritesCount { get; private set; }

        [DataMember(Name = "stauses_count")]
        public int StatusesCount { get; private set; }

        [DataMember(Name = "created_at")]
        [JsonFormatter(typeof(DateTimeOffsetFormatter))]
        public DateTimeOffset CreatedAt { get; private set; }

        [DataMember(Name = "utc_offset")]
        public int? UtcOffset { get; private set; }

        [DataMember(Name = "time_zone")]
        public string TimeZone { get; private set; }

        [DataMember(Name = "geo_enabled")]
        public bool IsGeoEnabled { get; private set; }

        [DataMember(Name = "lang")]
        public string Language { get; private set; }

        [DataMember(Name = "contributors_enabled")]
        public bool IsContributorsEnabled { get; private set; }

        [DataMember(Name = "profile_background_color")]
        public string ProfileBackgroundColor { get; private set; }

        [DataMember(Name = "profile_background_image_url")]
        public string ProfileBackgroundImageUrl { get; private set; }

        [DataMember(Name = "profile_background_image_url_https")]
        public string ProfileBackgroundImageUrlHttps { get; private set; }

        [DataMember(Name = "profile_background_tile")]
        public bool ProfileBackgroundTile { get; private set; }

        [DataMember(Name = "profile_banner_url")]
        public string ProfileBannerUrl { get; private set; }

        [DataMember(Name = "profile_image_url")]
        public string ProfileImageUrl { get; private set; }

        [DataMember(Name = "profile_image_url_https")]
        public string ProfileImageUrlHttps { get; private set; }

        [DataMember(Name = "profile_link_color")]
        public string ProfileLinkColor { get; private set; }

        [DataMember(Name = "profile_sidebar_border_color")]
        public string ProfileSidebarBorderColor { get; private set; }

        [DataMember(Name = "profile_sidebar_fill_color")]
        public string ProfileSidebarFillColor { get; private set; }

        [DataMember(Name = "profile_text_color")]
        public string ProfileTextColor { get; private set; }

        [DataMember(Name = "profile_use_background_Image")]
        public bool IsProfileUseBackgroundImage { get; private set; }

        [DataMember(Name = "default_profile")]
        public bool IsDefaultProfile { get; private set; }

        [DataMember(Name = "default_profile_image")]
        public bool IsDefaultProfileImage { get; private set; }

        [DataMember(Name = "withheld_in_countries")]
        public string WithheldInCountries { get; private set; }

        [DataMember(Name = "withheld_scope")]
        public string WithheldScope { get; private set; }
    }
}
