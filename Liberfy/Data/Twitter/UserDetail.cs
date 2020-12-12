using System;
using System.Collections.Generic;
using Liberfy.Model;
using Liberfy.Services.Twitter;
using Liberfy.Settings;
using SocialApis.Twitter;

namespace Liberfy.Data.Twitter
{
    /// <summary>
    /// アカウント情報
    /// </summary>
    internal class UserDetail : NotificationObject, IEquatable<UserDetail>, IUserInfo<User>
    {
        public ServiceType Service { get; } = ServiceType.Twitter;

        public long Id { get; }

        public Uri Instance { get; }

        private DateTimeOffset _createdAt;
        public DateTimeOffset CreatedAt => this._createdAt;

        private string _name;
        public string Name => this._name;

        private string _userName;
        public string UserName => this._userName;

        private string _fullName;
        public string FullName => this._fullName;

        private string _description;
        public string Description => this._description;

        private int _followersCount;
        public int FollowersCount => this._followersCount;

        private int _followingCount;
        public int FollowingsCount => this._followingCount;

        public string _language;
        public string Language => this._language;

        private string _location;
        public string Location => this._location;

        private string _profileBannerUrl;
        public string ProfileBannerUrl => this._profileBannerUrl;

        private string _profileImageUrl;
        public string ProfileImageUrl => this._profileImageUrl;

        private bool _isProtected;
        public bool IsProtected => this._isProtected;

        private int _statusCount;
        public int StatusesCount => this._statusCount;

        private bool _isSuspended;
        public bool IsSuspended => this._isSuspended;

        public string _url;
        public string Url => this._url;

        public string _remoteUrl;
        public string RemoteUrl => this.RemoteUrl;

        public DateTime UpdatedAt { get; private set; }

        private TwitterTextTokenBuilder _descriptionEntitiesTokenBuilder;
        private IEnumerable<IEntity> _descriptionEntities;
        public IEnumerable<IEntity> DescriptionEntities
        {
            get
            {
                if (this._descriptionEntities == null)
                {
                    this._descriptionEntities = this._descriptionEntitiesTokenBuilder.Build();
                    this._descriptionEntitiesTokenBuilder = null;
                }

                return this._descriptionEntities;
            }
        }

        private TwitterTextTokenBuilder _urlEntitiesTokenBuilder;
        private IEnumerable<IEntity> _urlEntities;
        public IEnumerable<IEntity> UrlEntities
        {
            get
            {
                if (this._urlEntities == null)
                {
                    this._urlEntities = this._urlEntitiesTokenBuilder.Build();
                    this._urlEntitiesTokenBuilder = null;
                }

                return this._urlEntities;
            }
        }

        public UserDetail(TwitterAccountSetting item)
        {
            this.Id = item.UserId;
            this._userName = item.ScreenName;
            this._name = item.Name;
            this._isProtected = item.IsProtected;
            this._profileImageUrl = item.ProfileImageUrl;
        }

        public UserDetail(User user)
        {
            this.Id = user.Id ?? throw new ArgumentNullException(nameof(user));
            this._createdAt = user.CreatedAt;
            this.Update(user);
        }

        public UserDetail Update(User user)
        {
            if (user.Id != this.Id)
                throw new ArgumentException(nameof(user.Id));

            const string RemoteUrlBase = "https://twitter.com/";

            var batch = new BatchPropertyChanges();

            batch.Set(ref this._createdAt, user.CreatedAt, nameof(this.CreatedAt));
            batch.Set(ref this._followersCount, user.FollowersCount, nameof(this.FollowersCount));
            batch.Set(ref this._followingCount, user.FriendsCount, nameof(this.FollowersCount));
            batch.Set(ref this._language, user.Language, nameof(this.Language));
            batch.Set(ref this._location, user.Location, nameof(this.Location));
            batch.Set(ref this._name, user.Name, nameof(this.Name));
            batch.Set(ref this._profileBannerUrl, user.ProfileBannerUrl, nameof(this.ProfileBannerUrl));
            batch.Set(ref this._profileImageUrl, user.ProfileImageUrl, nameof(this.ProfileImageUrl));
            batch.Set(ref this._isProtected, user.IsProtected, nameof(this.IsProtected));
            batch.Set(ref this._statusCount, user.StatusesCount, nameof(this.StatusesCount));
            batch.Set(ref this._remoteUrl, RemoteUrlBase + user.ScreenName, nameof(this.RemoteUrl));
            batch.Set(ref this._isSuspended, user.IsSuspended ?? false, nameof(this.IsSuspended));

            if (batch.Set(ref this._userName, user.ScreenName, nameof(this.UserName)) || this._fullName == null)
            {
                var longUserName = user.ScreenName + "@twitter.com";
                batch.Set(ref this._fullName, longUserName, nameof(this.FullName));
            }

            if (batch.Set(ref this._description, user.Description ?? string.Empty, nameof(this.Description)))
            {
                batch.Set(ref this._descriptionEntities, null, nameof(this.DescriptionEntities));
                this._descriptionEntitiesTokenBuilder = new TwitterTextTokenBuilder(this.Description, user.Entities?.Description);
            }

            if (batch.Set(ref this._url, user.Url ?? string.Empty, nameof(this.Url)))
            {
                batch.Set(ref this._urlEntities, null, nameof(this.UrlEntities));
                this._urlEntitiesTokenBuilder = new TwitterTextTokenBuilder(this.Url, user.Entities?.Url);
            }

            this.UpdatedAt = DateTime.Now;

            batch.Execute(this.RaisePropertyChanged);

            return this;
        }

        IUserInfo<User> IUserInfo<User>.Update(User user)
        {
            return this.Update(user);
        }

        public bool Equals(IUserInfo other)
        {
            return object.ReferenceEquals(this, other)
                || (other is UserDetail user && this.Equals(user));
        }

        public bool Equals(UserDetail other)
        {
            return this.Id == other.Id;
        }
    }
}
