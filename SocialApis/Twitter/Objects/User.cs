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
        private long _id;
        public long Id => _id;

        [DataMember(Name = "name")]
        private string _name;
        public string Name => _name;

        [DataMember(Name = "screen_name")]
        private string _screenName;
        public string ScreenName => _screenName;

        [DataMember(Name = "location")]
        private string _location;
        public string Location => _location;

        [DataMember(Name = "url")]
        private string _url;
        public string Url => _url;

        [DataMember(Name = "description")]
        private string _description;
        public string Description => _description;

        // [DataMember(Name = "derived")]

        [DataMember(Name = "protected")]
        private bool _protected;
        public bool IsProtected => _protected;

        [DataMember(Name = "verified")]
        private bool _verified;
        public bool IsVerified => _verified;

        [DataMember(Name = "followers_count")]
        private int _followersCount;
        public int FollowersCount => _followersCount;

        [DataMember(Name = "friends_count")]
        private int _friendsCount;
        public int FriendsCount => _friendsCount;

        [DataMember(Name = "listed_count")]
        private int _listedCount;
        public int ListedCount => _listedCount;

        [DataMember(Name = "favourites_count")]
        private int _favoritesCount;
        public int FavoritesCount => _favoritesCount;

        [DataMember(Name = "stauses_count")]
        private int _statusesCount;
        public int StatusesCount => _statusesCount;

        [DataMember(Name = "created_at")]
        [JsonFormatter(typeof(DateTimeOffsetFormatter))]
        private DateTimeOffset _createdAt;
        public DateTimeOffset CreatedAt => _createdAt;

        [DataMember(Name = "utc_offset")]
        private int? _utcOffset;
        public int? UtcOffset => _utcOffset;

        [DataMember(Name = "time_zone")]
        private string _timeZone;
        public string TimeZone => _timeZone;

        [DataMember(Name = "geo_enabled")]
        private bool _geoEnabled;
        public bool IsGeoEnabled => _geoEnabled;

        [DataMember(Name = "lang")]
        private string _language;
        public string Language => _language;

        [DataMember(Name = "contributors_enabled")]
        private bool _contributorsEnabled;
        public bool IsContributorsEnabled => _contributorsEnabled;

        [DataMember(Name = "profile_background_color")]
        private string _profileBackgroundColor;
        public string ProfileBackgroundColor => _profileBackgroundColor;

        [DataMember(Name = "profile_background_image_url")]
        private string _profileBackgroundImageUrl;
        public string ProfileBackgroundImageUrl => _profileBackgroundImageUrl;

        [DataMember(Name = "profile_background_image_url_https")]
        private string _profileBackgroundImageUrlHttps;
        public string ProfileBackgroundImageUrlHttps => _profileBackgroundImageUrlHttps;

        [DataMember(Name = "profile_background_tile")]
        private bool _profileBackgroundTile;
        public bool ProfileBackgroundTile => _profileBackgroundTile;

        [DataMember(Name = "profile_banner_url")]
        private string _profileBannerUrl;
        public string ProfileBannerUrl => _profileBannerUrl;

        [DataMember(Name = "profile_image_url")]
        private string _profileImageUrl;
        public string ProfileImageUrl => _profileImageUrl;

        [DataMember(Name = "profile_image_url_https")]
        private string _profileImageUrlHttps;
        public string ProfileImageUrlHttps => _profileImageUrlHttps;

        [DataMember(Name = "profile_link_color")]
        private string _profileLinkColor;
        public string ProfileLinkColor => _profileLinkColor;

        [DataMember(Name = "profile_sidebar_border_color")]
        private string _profileSidebarBorderColor;
        public string ProfileSidebarBorderColor => _profileSidebarBorderColor;

        [DataMember(Name = "profile_sidebar_fill_color")]
        private string _profileSidebarFillColor;
        public string ProfileSidebarFillColor => _profileSidebarBorderColor;

        [DataMember(Name = "profile_text_color")]
        private string _profileTextColor;
        public string ProfileTextColor => _profileTextColor;

        [DataMember(Name = "profile_use_background_Image")]
        private bool _profileUseBackgroundImage;
        public bool IsProfileUseBackgroundImage => _profileUseBackgroundImage;

        [DataMember(Name = "default_profile")]
        private bool _defaultProfile;
        public bool IsDefaultProfile => _defaultProfile;

        [DataMember(Name = "default_profile_image")]
        private bool _defaultProfileImage;
        public bool IsDefaultProfileImage => _defaultProfileImage;

        [DataMember(Name = "withheld_in_countries")]
        private string _withheldInCountries;
        public string WithheldInCountries => _withheldInCountries;

        [DataMember(Name = "withheld_scope")]
        private string _withheldScope;
        public string WithheldScope => _withheldScope;
    }
}
