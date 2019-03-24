using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Liberfy.Model;
using SocialApis;
using SocialApis.Common;

namespace Liberfy
{
    internal class UserInfo : NotificationObject, IEquatable<UserInfo>
    {
        public ServiceType Service { get; }

        public long Id { get; }

        public Uri Host { get; private set; }

        public DateTimeOffset CreatedAt { get; set; }

        private string _longUserName;
        public string LongUserName
        {
            get => this._longUserName;
            set => this.SetProperty(ref this._longUserName, value);
        }

        private string _description;
        public string Description
        {
            get => this._description;
            set => this.SetProperty(ref this._description, value);
        }

        private ITextEntityBuilder _descriptionEntitiesBuilder;
        private IEnumerable<IEntity> _descriptionEntities = new IEntity[0];
        public IEnumerable<IEntity> DescriptionEntities
        {
            get => this._descriptionEntities ?? (this._descriptionEntities = this._descriptionEntitiesBuilder.Build());
            set => this._descriptionEntities = value ?? new IEntity[0];
        }

        public void SetDescriptionEntitiesBuilder(ITextEntityBuilder builder)
        {
            this._descriptionEntitiesBuilder = builder;
        }

        private ITextEntityBuilder _urlEntitiesBuilder;
        private IEnumerable<IEntity> _urlEntities;
        public IEnumerable<IEntity> UrlEntities
        {
            get => this._urlEntities ?? (this._urlEntities = this._urlEntitiesBuilder.Build());
            set => this._urlEntities = value ?? new IEntity[0];
        }

        public void SetUrlEntitiesBuilder(ITextEntityBuilder builder)
        {
            this._urlEntitiesBuilder = builder;
        }

        private int _followersCount;
        public int FollowersCount
        {
            get => this._followersCount;
            set => this.SetProperty(ref this._followersCount, value);
        }

        private int _friendsCount;
        public int FriendsCount
        {
            get => this._friendsCount;
            set => this.SetProperty(ref this._friendsCount, value);
        }

        private string _language;
        public string Language
        {
            get => this._language;
            set => this.SetProperty(ref this._language, value);
        }

        private string _location;
        public string Location
        {
            get => this._location;
            set => this.SetProperty(ref this._location, value);
        }

        private string _name;
        public string Name
        {
            get => this._name;
            set => this.SetProperty(ref this._name, value);
        }

        private string _profileBannerUrl;
        public string ProfileBannerUrl
        {
            get => this._profileBannerUrl;
            set => this.SetProperty(ref this._profileBannerUrl, value);
        }

        private string _profileImageUrl;
        public string ProfileImageUrl
        {
            get => this._profileImageUrl;
            set => this.SetProperty(ref this._profileImageUrl, value);
        }

        private bool _isProtected;
        public bool IsProtected
        {
            get => this._isProtected;
            set => this.SetProperty(ref this._isProtected, value);
        }

        private string _screenName;
        public string ScreenName
        {
            get => this._screenName;
            set => this.SetProperty(ref this._screenName, value);
        }

        private int _statusesCount;
        public int StatusesCount
        {
            get => this._statusesCount;
            set => this.SetProperty(ref this._statusesCount, value);
        }

        private bool _isSuspended;
        public bool IsSuspended
        {
            get => this._isSuspended;
            set => this.SetProperty(ref this._isSuspended, value);
        }

        private string _url;
        public string Url
        {
            get => this._url;
            set => this.SetProperty(ref this._url, value);
        }

        private string _remoteUrl;
        public string RemoteUrl
        {
            get => this._remoteUrl;
            set => this.SetProperty(ref this._remoteUrl, value);
        }

        public DateTime UpdatedAt { get; set; }

        public UserInfo(ServiceType service, Uri host, long id)
        {
            this.Host = host;
            this.Id = id;
            this.Service = service;
        }

        public UserInfo(Uri host, long id, string name, string screenName, bool isProtected, string profileImageUrl)
        {
            this.Host = host;
            this.Id = id;
            this.Name = name;
            this.ScreenName = screenName;
            this.IsProtected = isProtected;
            this.ProfileImageUrl = profileImageUrl;

            this.UpdatedAt = DateTime.Now;
        }

        public bool Equals(UserInfo other) => Equals(this.Id, other?.Id) && this.Service == other.Service;

        public override bool Equals(object obj)
        {
            return obj is UserInfo userInfo && this.Equals(userInfo);
        }

        public override int GetHashCode() => this.Id.GetHashCode();
    }
}
