using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SocialApis.Twitter;

namespace Liberfy
{
    internal class UserInfo : NotificationObject, IObjectInfo<User>, IEquatable<UserInfo>, IEquatable<User>
    {
        public long Id { get; }
        public bool IsContributorsEnabled { get; private set; }
        public DateTimeOffset CreatedAt { get; }
        public bool IsDefaultProfile { get; private set; }
        public bool IsDefaultProfileImage { get; private set; }
        public string Description { get; private set; }
        public UserEntities Entities { get; private set; }
        public int FavouritesCount { get; private set; }
        public int FollowersCount { get; private set; }
        public int FriendsCount { get; private set; }
        // public bool HasExtendedProfile { get; private set; }
        public bool IsGeoEnabled { get; private set; }
        public bool IsTranslationEnabled { get; private set; }
        public string Language { get; private set; }
        public int ListedCount { get; private set; }
        public string Location { get; private set; }
        public string Name { get; private set; }
        public bool NeedsPhoneVerification { get; private set; }
        public string ProfileBackgroundImageUrl { get; private set; }
        public bool IsProfileBackgroundTile { get; private set; }
        public string ProfileBannerUrl { get; private set; }
        public string ProfileImageUrl { get; private set; }
        public Places ProfileLocation { get; private set; }
        public bool IsProfileUseBackgroundImage { get; private set; }
        public bool IsProtected { get; private set; }
        public string ScreenName { get; private set; }
        // public bool IsShowAllInlineMedia { get; private set; }
        public int StatusesCount { get; private set; }
        public bool IsSuspended { get; private set; }
        public string TimeZone { get; private set; }
        // public string TranslatorType { get; private set; }
        public string Url { get; private set; }
        public int UtcOffset { get; private set; }
        public bool IsVerified { get; private set; }
        public string WithheldInCountries { get; private set; }
        public string WithheldScope { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public UserInfo(User user)
        {
            this.Id = user.Id ?? Id;
            this.CreatedAt = user.CreatedAt;

            this.Update(user);
        }

        public UserInfo(long id, string name, string screenName, bool isProtected, string profileImageUrl)
        {
            this.Id = id;
            this.Name = name;
            this.ScreenName = screenName;
            this.IsProtected = isProtected;
            this.ProfileImageUrl = profileImageUrl;

            this.UpdatedAt = DateTime.Now;
        }

        public UserInfo Update(User item)
        {
            this.IsContributorsEnabled = item.IsContributorsEnabled;
            this.IsDefaultProfile = item.IsDefaultProfile;
            this.IsDefaultProfileImage = item.IsDefaultProfileImage;
            this.Description = item.Description;
            this.Entities = item.Entities;
            this.FavouritesCount = item.FavoritesCount;
            this.FollowersCount = item.FollowersCount;
            this.FriendsCount = item.FriendsCount;
            // this.HasExtendedProfile = item.HasExtendedProfile ?? this.HasExtendedProfile;
            this.IsGeoEnabled = item.IsGeoEnabled;
            this.IsTranslationEnabled = item.IsTranslationEnabled;
            this.Language = item.Language;
            this.ListedCount = item.ListedCount ?? this.ListedCount;
            this.Location = item.Location;
            this.Name = item.Name;
            this.NeedsPhoneVerification = item.NeedsPhoneVerification ?? NeedsPhoneVerification;
            this.ProfileBackgroundImageUrl = item.ProfileBackgroundImageUrl;
            this.IsProfileBackgroundTile = item.ProfileBackgroundTile;
            this.ProfileBannerUrl = item.ProfileBannerUrl;
            this.ProfileImageUrl = item.ProfileImageUrl;
            this.ProfileLocation = item.ProfileLocation;
            this.IsProfileUseBackgroundImage = item.IsProfileUseBackgroundImage;
            this.IsProtected = item.IsProtected;
            this.ScreenName = item.ScreenName;
            // this.IsShowAllInlineMedia = item.IsShowAllInlineMedia ?? IsShowAllInlineMedia;
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
