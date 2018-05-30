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

        public DateTimeOffset CreatedAt { get; }

        private bool _isContributorsEnabled;
        public bool IsContributorsEnabled
        {
            get => this._isContributorsEnabled;
            private set => this.SetProperty(ref this._isContributorsEnabled, value);
        }

        private bool _isDefaultProfile;
        public bool IsDefaultProfile
        {
            get => this._isDefaultProfile;
            private set => this.SetProperty(ref this._isDefaultProfile, value);
        }

        private bool _isDefaultProfileImage;
        public bool IsDefaultProfileImage
        {
            get => this._isDefaultProfileImage;
            private set => this.SetProperty(ref this._isDefaultProfileImage, value);
        }

        private string _description;
        public string Description
        {
            get => this._description;
            private set => this.SetProperty(ref this._description, value);
        }

        private UserEntities _entities;
        public UserEntities Entities
        {
            get => this._entities;
            private set => this.SetProperty(ref this._entities, value);
        }

        private int _favouritesCount;
        public int FavouritesCount
        {
            get => this._favouritesCount;
            private set => this.SetProperty(ref this._favouritesCount, value);
        }

        private int _followersCount;
        public int FollowersCount
        {
            get => this._followersCount;
            private set => this.SetProperty(ref this._followersCount, value);
        }

        private int _friendsCount;
        public int FriendsCount
        {
            get => this._friendsCount;
            private set => this.SetProperty(ref this._friendsCount, value);
        }
        // public bool HasExtendedProfile { get; private set; }

        private bool _isGeoEnabled;
        public bool IsGeoEnabled
        {
            get => this._isGeoEnabled;
            private set => this.SetProperty(ref this._isGeoEnabled, value);
        }

        private bool _isTranslationEnabled;
        public bool IsTranslationEnabled
        {
            get => this._isTranslationEnabled;
            private set => this.SetProperty(ref this._isTranslationEnabled, value);
        }

        private string _language;
        public string Language
        {
            get => this._language;
            private set => this.SetProperty(ref this._language, value);
        }

        private int _listCount;
        public int ListedCount
        {
            get => this._listCount;
            private set => this.SetProperty(ref this._listCount, value);
        }

        private string _location;
        public string Location
        {
            get => this._location;
            private set => this.SetProperty(ref this._location, value);
        }

        private string _name;
        public string Name
        {
            get => this._name;
            private set => this.SetProperty(ref this._name, value);
        }

        private bool _needsPhoneVerification;
        public bool NeedsPhoneVerification
        {
            get => this._needsPhoneVerification;
            private set => this.SetProperty(ref this._needsPhoneVerification, value);
        }

        private string _profileBackgroundImageUrl;
        public string ProfileBackgroundImageUrl
        {
            get => this._profileBackgroundImageUrl;
            private set => this.SetProperty(ref this._profileBackgroundImageUrl, value);
        }

        private string _profileBannerUrl;
        public string ProfileBannerUrl
        {
            get => this._profileBannerUrl;
            private set => this.SetProperty(ref this._profileBannerUrl, value);
        }

        private string _profileImageUrl;
        public string ProfileImageUrl
        {
            get => this._profileImageUrl;
            private set => this.SetProperty(ref this._profileImageUrl, value);
        }

        private Places _profileLocation;
        public Places ProfileLocation
        {
            get => this._profileLocation;
            private set => this.SetProperty(ref this._profileLocation, value);
        }

        private bool _isProfileUseBackgroundImage;
        public bool IsProfileUseBackgroundImage
        {
            get => this._isProfileUseBackgroundImage;
            private set => this.SetProperty(ref this._isProfileUseBackgroundImage, value);
        }

        private bool _isProtected;
        public bool IsProtected
        {
            get => this._isProtected;
            private set => this.SetProperty(ref this._isProtected, value);
        }

        private string _screenName;
        public string ScreenName
        {
            get => this._screenName;
            private set => this.SetProperty(ref this._screenName, value);
        }

        // public bool IsShowAllInlineMedia { get; private set; }

        private int _statusesCount;
        public int StatusesCount
        {
            get => this._statusesCount;
            private set => this.SetProperty(ref this._statusesCount, value);
        }

        private bool _isSuspended;
        public bool IsSuspended
        {
            get => this._isSuspended;
            private set => this.SetProperty(ref this._isSuspended, value);
        }

        private string _timeZone;
        public string TimeZone
        {
            get => this._timeZone;
            private set => this.SetProperty(ref this._timeZone, value);
        }

        // public string TranslatorType { get; private set; }

        private string _url;
        public string Url
        {
            get => this._url;
            private set => this.SetProperty(ref this._url, value);
        }

        private bool _isVerified;
        public bool IsVerified
        {
            get => this._isVerified;
            private set => this.SetProperty(ref this._isVerified, value);
        }

        private string _withheldInCountires;
        public string WithheldInCountries
        {
            get => this._withheldInCountires;
            private set => this.SetProperty(ref this._withheldInCountires, value);
        }

        private string _withheldScope;
        public string WithheldScope
        {
            get => this._withheldScope;
            private set => this.SetProperty(ref this._withheldScope, value);
        }

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
            this.IsVerified = item.IsVerified;
            this.WithheldInCountries = item.WithheldInCountries;
            this.WithheldScope = item.WithheldScope;

            this.UpdatedAt = DateTime.Now;

            return this;
        }

        void IObjectInfo<User>.Update(User item) => this.Update(item);

        public bool Equals(UserInfo other) => Equals(this.Id, other?.Id);

        public bool Equals(User other) => Equals(this.Id, other?.Id);

        public override bool Equals(object obj)
        {
            return (obj is UserInfo userInfo && this.Equals(userInfo))
                || (obj is User user && this.Equals(user));
        }

        public override int GetHashCode() => this.Id.GetHashCode();
    }
}
