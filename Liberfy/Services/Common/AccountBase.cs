using Liberfy.Commands;
using Liberfy.Services;
using Liberfy.Services.Common;
using Liberfy.Settings;
using System;
using System.Threading.Tasks;

namespace Liberfy
{
    internal abstract class AccountBase : NotificationObject
    {
        public static IAccount FromSetting(IAccountSetting item)
            => item switch
            {
                TwitterAccountSetting twitterItem => new TwitterAccount(twitterItem),
                MastodonAccountSetting mastodonItem => new MastodonAccount(mastodonItem),
                _ => throw new NotImplementedException(),
            };

        public string ItemId { get; protected set; }

        public abstract ServiceType Service { get; }

        public abstract IApiGateway ApiGateway { get; }

        public abstract IServiceConfiguration ServiceConfiguration { get; }

        protected static Setting Setting { get; } = App.Setting;

        protected object LockSharedObject = new object();

        public AccountCommandGroup Commands { get; protected set; }

        protected abstract void OnActivityStarted();

        protected abstract void OnActivityStopping();

        private bool _isLoggedIn;

        public bool IsVerified
        {
            get => this._isLoggedIn;
            private set => this.SetProperty(ref this._isLoggedIn, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => this._isLoading;
            private set => this.SetProperty(ref this._isLoading, value);
        }

        protected AccountBase(bool isLoggedIn)
        {
            this._isLoggedIn = true;
        }

        public async Task StartActivity()
        {
            if (this.IsVerified || await this.Login().ConfigureAwait(false))
            {
                this.OnActivityStarted();
            }
        }

        protected async ValueTask<bool> Login()
        {
            this.IsLoading = true;

            bool loggedIn = await this.VerifyCredentials().ConfigureAwait(false);
            if (loggedIn)
            {
                this.IsVerified = true;
            }

            this.IsLoading = false;

            return loggedIn;
        }

        protected virtual ValueTask<bool> VerifyCredentials() => new(true);

        public void StopActivity()
        {
            this.OnActivityStopping();
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get => this._errorMessage;
            private set => this.SetProperty(ref this._errorMessage, value);
        }

        protected void SetErrorMessage(string name, string message)
        {
            lock (this.LockSharedObject)
            {
                var beforeStr = string.IsNullOrEmpty(this.ErrorMessage) ? string.Empty : "\n";

                this.ErrorMessage += $"{ beforeStr }{ name }の取得に失敗しました：\n{ message }";
            }
        }

        public abstract IValidator Validator { get; }

        public abstract IAccountSetting GetSetting();
    }
}
