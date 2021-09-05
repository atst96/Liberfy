using Liberfy.Commands;
using Liberfy.Data.Twitter;
using Liberfy.Managers;
using Liberfy.Services;
using Liberfy.Services.Common;
using Liberfy.Services.Twitter;
using Liberfy.Services.Twitter.Accessors;
using Liberfy.Settings;
using SocialApis;
using SocialApis.Twitter;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class TwitterAccount : AccountBase, IAccount
    {
        public static readonly Uri ServerHostUrl = new Uri("https://twitter.com/", UriKind.Absolute);
        private static readonly TwitterDataManager _dataFactory = new TwitterDataManager();

        private readonly TwitterAccountSetting _setting;

        public override ServiceType Service { get; } = ServiceType.Twitter;

        public TwitterDataManager DataStore { get; }

        public UserDetail Info { get; }

        public TwitterTimeline Timeline { get; }

        public TwitterApi Api { get; protected set; }

        public TwitterAccount(TwitterAccountSetting setting)
            : base(false)
        {
            this._setting = setting.Clone();
            this.ItemId = setting.ItemId;
            this.Api = setting.CreateApi();
            this.DataStore = _dataFactory;
            this.Info = this.DataStore.GetAccount(setting);
            this.Validator = new TwitterValidator(this);
            this.Timeline = new TwitterTimeline(this);
            this.Commands = new AccountCommandGroup(this);

            ((INotifyPropertyChanged)this.Info).PropertyChanged += this.OnProfileUpdated;
        }

        public TwitterAccount(string itemId, TwitterApi api, User account)
            : base(true)
        {
            this._setting = new()
            {
                ItemId = itemId,
                UserId = api.UserId,
            };
            this.ItemId = itemId;
            this.SetApiTokens(api);
            this.DataStore = _dataFactory;
            this.Info = this.DataStore.RegisterAccount(account);
            this.Validator = new TwitterValidator(this);
            this.Timeline = new TwitterTimeline(this);
            this.Commands = new AccountCommandGroup(this);
            this.OnProfileUpdated(this.Info, new(null));

            ((INotifyPropertyChanged)this.Info).PropertyChanged += this.OnProfileUpdated;
        }

        //private IAccountCommandGroup _commands;
        //public override IAccountCommandGroup Commands => _commands ?? (_commands = new Twitter.AccountCommandGroup(this));

        public override IValidator Validator { get; }

        public override IServiceConfiguration ServiceConfiguration { get; } = new TwitterServiceConfiguration();

        private IApiGateway _apiGateway;
        public override IApiGateway ApiGateway => this._apiGateway ??= new TwitterApiGateway(this);

        private TwitterStatusAccessor _statuses;
        private TwitterMediaAccessor _media;

        /// <summary>
        /// ツイート関連のアクセサ
        /// </summary>
        public TwitterStatusAccessor Statuses => this._statuses ??= new TwitterStatusAccessor(this);

        /// <summary>
        /// メディア関連のアクセサ
        /// </summary>
        public TwitterMediaAccessor Media => this._media ??= new TwitterMediaAccessor(this);

        protected override async ValueTask<bool> VerifyCredentials()
        {
            try
            {
                var user = await this.Api.Account.VerifyCredentials(new Query
                {
                    ["include_entities"] = true,
                    ["skip_status"] = true,
                    ["include_email"] = false
                });

                this.DataStore.RegisterAccount(user);

                return true;
            }
            catch (TwitterException tex)
            {
                Console.WriteLine(tex.Message);

                if (tex.InnerException is WebException wex)
                {
                    switch (wex.Status)
                    {
                        case WebExceptionStatus.Success:
                            break;
                    }
                }
            }

            return false;
        }

        public void SetApiTokens(TwitterApi api)
        {
            this.Api = api;

            var setting = this._setting;
            setting.ConsumerKey = api.ConsumerKey;
            setting.ConsumerSecret = api.ConsumerSecret;
            setting.AccessToken = api.AccessToken;
            setting.AccessTokenSecret = api.AccessTokenSecret;
        }

        private void OnProfileUpdated(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case null:
                case "*":
                case nameof(UserDetail.Name):
                case nameof(UserDetail.UserName):
                case nameof(UserDetail.IsProtected):
                case nameof(UserDetail.ProfileImageUrl):
                    var setting = this._setting;
                    var user = this.Info;

                    setting.Name = user.Name;
                    setting.ScreenName = user.UserName;
                    setting.ProfileImageUrl = user.ProfileImageUrl;
                    setting.IsProtected = user.IsProtected;
                    break;
            }
        }

        public override IAccountSetting GetSetting() => this._setting.Clone();

        protected readonly ConcurrentDictionary<long, StatusActivity> _statusActivities = new();

        public StatusActivity GetStatusActivity(long originalStatusId)
        {
            return this._statusActivities.GetOrAdd(originalStatusId, _ => new());
        }

        protected override void OnActivityStarted()
        {
            //this.GetDetails();

            if (Config.Functions.IsLoadTimelineEnabled)
            {
                this.Timeline.Load();
            }
        }

        protected override void OnActivityStopping()
        {
            this.Timeline.Unload();
            this._statusActivities.Clear();
        }
    }
}
