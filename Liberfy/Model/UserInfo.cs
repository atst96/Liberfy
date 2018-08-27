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
        public SocialService Service { get; }

        public long Id { get; }

        public DateTimeOffset CreatedAt { get; private set; }

        private string _longUserName;
        public string LongUserName
        {
            get => this._longUserName;
            private set => this.SetProperty(ref this._longUserName, value);
        }

        private string _description;
        public string Description
        {
            get => this._description;
            private set => this.SetProperty(ref this._description, value);
        }

        private ITextEntityBuilder _descriptionEntitiesBuilder;
        private IEnumerable<EntityBase> _descriptionEntities;
        public IEnumerable<EntityBase> DescriptionEntities
        {
            get
            {
                if (this._descriptionEntities == null)
                    this._descriptionEntities = this._descriptionEntitiesBuilder.Build();

                return this._descriptionEntities;
            }
        }

        private ITextEntityBuilder _urlEntitiesBuilder;
        public IEnumerable<EntityBase> _urlEntities;
        public IEnumerable<EntityBase> UrlEntities
        {
            get
            {
                if (this._urlEntities == null)
                    this._urlEntities = this._urlEntitiesBuilder.Build();

                return this._urlEntities;
            }
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

        private string _language;
        public string Language
        {
            get => this._language;
            private set => this.SetProperty(ref this._language, value);
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

        private string _url;
        public string Url
        {
            get => this._url;
            private set => this.SetProperty(ref this._url, value);
        }

        private string _remoteUrl;
        public string RemoteUrl
        {
            get => this._remoteUrl;
            private set => this.SetProperty(ref this._remoteUrl, value);
        }

        public DateTime UpdatedAt { get; private set; }

        public UserInfo(IAccount account)
        {
            this.Service = account.Service;

            this.Id = account.Id ?? this.Id;
            this.CreatedAt = account.CreatedAt;

            this.Update(account);
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

        public UserInfo Update(SocialApis.Twitter.User item)
        {
            const string RemoteUrlBase = "https://twitter.com/";

            this.CreatedAt = item.CreatedAt;
            this.LongUserName = item.ScreenName;
            this.Description = item.Description;

            this._descriptionEntitiesBuilder = new TwitterTextTokenBuilder(item.Description ?? "", item.Entities?.Description);
            this._urlEntitiesBuilder = new TwitterTextTokenBuilder(item.Url ?? "", item.Entities?.Url);

            this.Url = item.Url;

            this.FollowersCount = item.FollowersCount;
            this.FriendsCount = item.FriendsCount;
            this.Language = item.Language;
            this.Location = item.Location;
            this.Name = item.Name;
            this.ProfileBannerUrl = item.ProfileBannerUrl;
            this.ProfileImageUrl = item.ProfileImageUrl;
            this.IsProtected = item.IsProtected;
            this.ScreenName = item.ScreenName;
            this.StatusesCount = item.StatusesCount;
            this.RemoteUrl = RemoteUrlBase + item.ScreenName;

            this.UpdatedAt = DateTime.Now;

            return this;
        }

        public UserInfo Update(SocialApis.Mastodon.Account item)
        {
            this.CreatedAt = item.CreatedAt;
            this.LongUserName = item.Acct;
            this.Description = item.Note;

            this._descriptionEntities = new EntityBase[0];

            this.Url = item.Url;

            this._urlEntities = new[] { new PlainTextEntity(item.Url, 0, item.Url.Length) };

            this.FollowersCount = item.FollowersCount;
            this.FriendsCount = item.FollowersCount;
            //this.Language = item.Language;
            //this.Location = item.Location;
            this.Name = item.DisplayName;
            this.ProfileBannerUrl = item.Header;
            this.ProfileImageUrl = item.Avatar;
            this.IsProtected = item.IsLocked;
            this.ScreenName = item.UserName;
            this.StatusesCount = item.StatusesCount;
            this.RemoteUrl = item.Url;

            this.UpdatedAt = DateTime.Now;

            return this;
        }

        public UserInfo Update(IAccount item)
        {
            if (item is SocialApis.Twitter.User twUser)
            {
                return this.Update(twUser);
            }
            else if (item is SocialApis.Mastodon.Account mdUser)
            {
                return this.Update(mdUser);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool Equals(UserInfo other) => Equals(this.Id, other?.Id) && this.Service == other.Service;

        public override bool Equals(object obj)
        {
            return (obj is UserInfo userInfo && this.Equals(userInfo));
        }

        public override int GetHashCode() => this.Id.GetHashCode();
    }
}
