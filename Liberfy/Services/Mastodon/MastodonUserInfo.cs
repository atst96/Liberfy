using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Model;
using Liberfy.Settings;
using SocialApis.Mastodon;

namespace Liberfy
{
    internal class MastodonUserInfo : NotificationObject, IEquatable<MastodonUserInfo>, IUserInfo<Account>
    {
        public ServiceType Service { get; } = ServiceType.Mastodon;

        public long Id { get; }

        public Uri Host { get; }

        private DateTimeOffset _createdAt;
        public DateTimeOffset CreatedAt => this._createdAt;

        private string _longUserName;
        public string LongUserName => this._longUserName;

        private string _description;
        public string Description => this._description;

        private int _followersCount;
        public int FollowersCount => this._followersCount;

        private int _followingsCount;
        public int FollowingsCount => this._followingsCount;

        public string _language;
        public string Language => this._language;

        private string _location;
        public string Location => this._location;

        private string _name;
        public string Name => this._name;

        private string _profileBannerUrl;
        public string ProfileBannerUrl => this._profileBannerUrl;

        private string _profileImageUrl;
        public string ProfileImageUrl => this._profileImageUrl;

        private bool _isProtected;
        public bool IsProtected => this._isProtected;

        private string _screenName;
        public string ScreenName => this._screenName;

        private int _statusCount;
        public int StatusesCount => this._statusCount;

        private bool _isSuspended;
        public bool IsSuspended => this._isSuspended;

        public string _url;
        public string Url => this._url;

        public string _remoteUrl;
        public string RemoteUrl => this.RemoteUrl;

        public DateTime UpdatedAt { get; private set; }

        private IEnumerable<IEntity> _descriptionEntities;
        public IEnumerable<IEntity> DescriptionEntities => this._descriptionEntities;

        private IEnumerable<IEntity> _urlEntities;
        public IEnumerable<IEntity> UrlEntities => this._urlEntities;

        public MastodonUserInfo(Uri host, AccountItem item)
        {
            this.Host = host;

            this.Id = item.Id;
            this._screenName = item.ScreenName;
            this._name = item.Name;
            this._isProtected = item.IsProtected;
            this._profileImageUrl = item.ProfileImageUrl;
        }

        public MastodonUserInfo(Uri host, Account account)
        {
            this.Host = host;

            this.Id = account.Id;
            this._createdAt = account.CreatedAt;

            this.Update(account);
        }

        public MastodonUserInfo Update(Account account)
        {
            var batch = new BatchPropertyChanges();

            batch.Set(ref this._longUserName, account.Acct, nameof(this.LongUserName));
            batch.Set(ref this._followersCount, account.FollowersCount, nameof(this.FollowersCount));
            batch.Set(ref this._followingsCount, account.FollowingCount, nameof(this.FollowingsCount));
            //batch.Set(ref this._language, account.Language, nameof(this.Language));
            //batch.Set(ref this._location, account.Location, nameof(this.Location));
            batch.Set(ref this._name, account.DisplayName, nameof(this.Name));
            batch.Set(ref this._profileBannerUrl, account.Header, nameof(this.ProfileBannerUrl));
            batch.Set(ref this._profileImageUrl, account.Avatar, nameof(this.ProfileImageUrl));
            batch.Set(ref this._isProtected, account.IsLocked, nameof(this.IsProtected));
            batch.Set(ref this._screenName, account.UserName, nameof(this.ScreenName));
            batch.Set(ref this._statusCount, account.StatusesCount, nameof(this.StatusesCount));
            batch.Set(ref this._remoteUrl, account.Url, nameof(this.RemoteUrl));

            if (batch.Set(ref this._description, account.Note, nameof(this.Description)))
            {
                var entities = new[] { new PlainTextEntity(this.Description) };
                batch.Set(ref this._descriptionEntities, entities, nameof(this.DescriptionEntities));
            }

            if (batch.Set(ref this._url, account.Url, nameof(this.Url)))
            {
                var entities = new[] { new PlainTextEntity(this.Url) };
                batch.Set(ref this._urlEntities, entities, nameof(this.UrlEntities));
            }

            this.UpdatedAt = DateTime.Now;

            batch.Execute(this.RaisePropertyChanged);

            return this;
        }

        IUserInfo<Account> IUserInfo<Account>.Update(Account user)
        {
            return this.Update(user);
        }

        public bool Equals(IUserInfo other)
        {
            return object.ReferenceEquals(this, other)
                || (other is MastodonAccount account && this.Equals(account));
        }

        public bool Equals(MastodonUserInfo other)
        {
            return this.Host == other.Host && this.Id == other.Id;
        }
    }
}
