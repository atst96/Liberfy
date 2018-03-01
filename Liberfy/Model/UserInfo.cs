using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using System.Windows.Media;

namespace Liberfy
{
    internal class UserInfo : NotificationObject, IObjectInfo<User>, IEquatable<UserInfo>, IEquatable<User>
    {
        public bool IsContributorsEnabled { get; private set; }
        public DateTimeOffset CreatedAt { get; }
        public bool IsDefaultProfile { get; private set; }
        public bool IsDefaultProfileImage { get; private set; }
        public string Description { get; private set; }
        public UserEntities Entities { get; private set; }
        public int FavouritesCount { get; private set; }
        public bool IsFollowRequestSent { get; private set; }
        public int FollowersCount { get; private set; }
        public int FriendsCount { get; private set; }
        public bool HasExtendedProfile { get; private set; }
        public bool IsGeoEnabled { get; private set; }
        public long Id { get; }
        public bool IsTranslator { get; private set; }
        public bool IsTranslationEnabled { get; private set; }
        public string Language { get; private set; }
        public int ListedCount { get; private set; }
        public string Location { get; private set; }
        public bool IsMuting { get; private set; }
        public string Name { get; private set; }
        public bool NeedsPhoneVerification { get; private set; }
        public Color ProfileBackgroundColor { get; private set; }
        public string ProfileBackgroundImageUrl { get; private set; }
        public string ProfileBackgroundImageUrlHttps { get; private set; }
        public bool IsProfileBackgroundTile { get; private set; }
        public string ProfileBannerUrl { get; private set; }
        public string ProfileImageUrl { get; private set; }
        public Color ProfileLinkColor { get; private set; }
        public Place ProfileLocation { get; private set; }
        public Color ProfileSidebarBorderColor { get; private set; }
        public Color ProfileSidebarFillColor { get; private set; }
        public Color ProfileTextColor { get; private set; }
        public bool IsProfileUseBackgroundImage { get; private set; }
        public bool IsProtected { get; private set; }
        public string ScreenName { get; private set; }
        public bool IsShowAllInlineMedia { get; private set; }
        public int StatusesCount { get; private set; }
        public bool IsSuspended { get; private set; }
        public string TimeZone { get; private set; }
        public string TranslatorType { get; private set; }
        public string Url { get; private set; }
        public int UtcOffset { get; private set; }
        public bool IsVerified { get; private set; }
        public string WithheldInCountries { get; private set; }
        public string WithheldScope { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        long IObjectInfo<User>.Id => throw new NotImplementedException();

        public UserInfo(User user)
        {
            Id = user.Id ?? Id;
            CreatedAt = user.CreatedAt;

            Update(user);
        }

        public UserInfo(long id, string name, string screenName, bool isProtected, string profileImageUrl)
        {
            Id = id;
            Name = name;
            ScreenName = screenName;
            IsProtected = isProtected;
            ProfileImageUrl = profileImageUrl;

            UpdatedAt = DateTime.Now;
        }

        private static Color ToColor(string name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? Colors.Transparent
                : (Color)ColorConverter.ConvertFromString("#" + name);
        }

        public UserInfo Update(User item)
        {
            this.IsContributorsEnabled = item.IsContributorsEnabled;
            this.IsDefaultProfile = item.IsDefaultProfile;
            this.IsDefaultProfileImage = item.IsDefaultProfileImage;
            this.Description = item.Description;
            this.Entities = item.Entities;
            this.FavouritesCount = item.FavouritesCount;
            this.IsFollowRequestSent = item.IsFollowRequestSent ?? IsFollowRequestSent;
            this.FollowersCount = item.FollowersCount;
            this.FriendsCount = item.FriendsCount;
            // this.HasExtendedProfile = item.HasExtendedProfile ?? HasExtendedProfile;
            this.IsGeoEnabled = item.IsGeoEnabled;
            this.IsTranslator = item.IsTranslator;
            this.IsTranslationEnabled = item.IsTranslationEnabled;
            this.Language = item.Language;
            this.ListedCount = item.ListedCount ?? ListedCount;
            this.Location = item.Location;
            this.IsMuting = item.IsMuting ?? IsMuting;
            this.Name = item.Name;
            this.NeedsPhoneVerification = item.NeedsPhoneVerification ?? NeedsPhoneVerification;
            this.ProfileBackgroundColor = ToColor(item.ProfileBackgroundColor);
            this.ProfileBackgroundImageUrl = item.ProfileBackgroundImageUrl;
            this.ProfileBackgroundImageUrlHttps = item.ProfileBackgroundImageUrlHttps;
            this.IsProfileBackgroundTile = item.IsProfileBackgroundTile;
            this.ProfileBannerUrl = item.ProfileBannerUrl;
            this.ProfileImageUrl = item.ProfileImageUrl;
            this.ProfileLinkColor = ToColor(item.ProfileLinkColor);
            this.ProfileLocation = item.ProfileLocation;
            this.ProfileSidebarBorderColor = ToColor(item.ProfileSidebarBorderColor);
            this.ProfileSidebarFillColor = ToColor(item.ProfileSidebarFillColor);
            this.ProfileTextColor = ToColor(item.ProfileTextColor);
            this.IsProfileUseBackgroundImage = item.IsProfileUseBackgroundImage;
            this.IsProtected = item.IsProtected;
            this.ScreenName = item.ScreenName;
            this.IsShowAllInlineMedia = item.IsShowAllInlineMedia ?? IsShowAllInlineMedia;
            this.StatusesCount = item.StatusesCount;
            this.IsSuspended = item.IsSuspended ?? IsSuspended;
            this.TimeZone = item.TimeZone;
            // this.TranslatorType = item.TranslatorType;
            this.Url = item.Url;
            this.UtcOffset = item.UtcOffset ?? UtcOffset;
            this.IsVerified = item.IsVerified;
            this.WithheldInCountries = item.WithheldInCountries;
            this.WithheldScope = item.WithheldScope;

            this.UpdatedAt = DateTime.Now;

            this.RaisePropertyChanged("");

            return this;
        }

        void IObjectInfo<User>.Update(User item) => this.Update(item);

        public bool Equals(UserInfo other) => Equals(Id, other?.Id);

        public bool Equals(User other) => Equals(Id, other?.Id);

        public override bool Equals(object obj)
        {
            return (obj is UserInfo userInfo && Equals(userInfo))
                || (obj is User user && Equals(user));
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
