using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Liberfy.Managers;
using SocialApis.Twitter;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels.Authentications
{
    /// <summary>
    /// <see cref="Views.Authentications.TwitterAuthenticationView"/>のViewModel
    /// </summary>
    internal sealed class TwitterAuthenticationViewModel : NotificationObject
    {
        /// <summary>
        /// 認証失敗時
        /// </summary>
        public EventHandler<AuthenticationFailedMessage> AuthenticationFailed;

        /// <summary>
        /// 認証キャンセル
        /// </summary>
        public EventHandler Cancelled;

        /// <summary>
        /// 認証完了時
        /// </summary>
        public EventHandler Completed;

        /// <summary>
        /// ページのインデックス
        /// </summary>
        private static class PageIndices
        {
            /// <summary>
            /// トップページ
            /// </summary>
            public const int Top = 0;

            /// <summary>
            /// PINコード入力ページ
            /// </summary>
            public const int PinCode = 1;

            /// <summary>
            /// 認証完了ページ
            /// </summary>
            public const int Complete = 2;
        }

        /// <summary>
        /// 設定情報
        /// </summary>
        private Setting _settings;

        /// <summary>
        /// アカウント情報
        /// </summary>
        private AccountManager _accounts;

        /// <summary>
        /// Twitter API
        /// </summary>
        private TwitterApi _api;

        private TwitterAccount _account;

        /// <summary>
        /// アカウント情報
        /// </summary>
        public TwitterAccount Account
        {
            get => this._account;
            private set => this.SetProperty(ref this._account, ref value);
        }

        private RequestTokenResponse _requestToken;

        /// <summary>
        /// <see cref="TwitterAuthenticationViewModel"/>を生成する
        /// </summary>
        public TwitterAuthenticationViewModel() : base()
        {
            this._settings = App.Setting;
            this._accounts = App.Accounts;
        }

        private readonly PropertyChangedEventArgs _pinCodeProperty = new(nameof(PinCode));
        private string _pinCode;
        /// <summary>
        /// PINコード
        /// </summary>
        public string PinCode
        {
            get => this._pinCode;
            set
            {
                if (this.SetProperty(ref this._pinCode, ref value, this._pinCodeProperty))
                {
                    this.NextCommand.RaiseCanExecute();
                }
            }
        }

        private readonly PropertyChangedEventArgs _authorizeUrlProperty = new(nameof(AuthorizeUrl));
        private string _authorizeUrl;
        /// <summary>
        /// 認証URL
        /// </summary>
        public string AuthorizeUrl
        {
            get => this._authorizeUrl;
            set => this.SetProperty(ref this._authorizeUrl, ref value, this._authorizeUrlProperty);
        }

        private readonly PropertyChangedEventArgs _isBusyProperty = new(nameof(IsBusy));
        private bool _isBusy;
        /// <summary>
        /// ビジー状態のフラグ
        /// </summary>
        public bool IsBusy
        {
            get => this._isBusy;
            private set
            {
                if (this.SetProperty(ref this._isBusy, ref value, this._isBusyProperty))
                {
                    this.NextCommand.RaiseCanExecute();
                    this.CancelCommand.RaiseCanExecute();
                }
            }
        }

        private readonly PropertyChangedEventArgs _pageIndexProperty = new(nameof(PageIndex));
        private int _pageIndex = PageIndices.Top;
        /// <summary>
        /// ページのインデックス
        /// </summary>
        public int PageIndex
        {
            get => this._pageIndex;
            private set
            {
                if (this.SetProperty(ref this._pageIndex, ref value, this._pageIndexProperty))
                {
                    this.NextCommand.RaiseCanExecute();
                    this.CancelCommand.RaiseCanExecute();
                }
            }
        }

        private Command _nextCommand;
        /// <summary>
        /// 次ページコマンド
        /// </summary>
        public Command NextCommand => this._nextCommand ??= new AsyncDelegateCommand(this.NavigateToNextPage, this.CanNavigateToNextPage);

        /// <summary>
        /// 次のページに移動する
        /// </summary>
        /// <returns></returns>
        private Task NavigateToNextPage()
        {
            if (this.PageIndex == PageIndices.Top)
            {
                return this.GetAuthorizeUrl();
            }
            else if (this.PageIndex == PageIndices.PinCode)
            {
                return this.Authorize();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 認証画面のURLを取得し、表示する
        /// </summary>
        /// <returns></returns>
        private async Task GetAuthorizeUrl()
        {
            this.IsBusy = true;

            this._api ??= new(Config.Twitter.ConsumerKey, Config.Twitter.ConsumerSecret);

            try
            {
                this._requestToken = await this._api.OAuth.RequestToken();
            }
            catch (Exception ex)
            {
                // HACK: logging
                ex.DumpDebug();

                this.AuthenticationFailed?.Invoke(this, new()
                {
                    Instruction = "認証失敗",
                    Message = "認証URLの取得に失敗しました。\nしばらく時間をおいてから再度お試しください。",
                });
                this.Cancelled?.Invoke(this, new());
                return;
            }

            this.AuthorizeUrl = this._requestToken.GetAuthorizeUrl();
            App.Open(this.AuthorizeUrl);

            this.PageIndex = PageIndices.PinCode;

            this.IsBusy = false;
        }

        /// <summary>
        /// 入力されたPINコードを用いてアカウントの認証を行う
        /// </summary>
        /// <returns></returns>
        private async Task Authorize()
        {
            var pinCode = this.PinCode.ToLower().Trim();

            this.IsBusy = true;

            this._api = await this._api.OAuth.AccessToken(this._requestToken, pinCode);

            UserResponse account;
            try
            {
                account = await this._api.Account.VerifyCredentials();
            }
            catch (Exception ex)
            {
                // TODO: ログ
                ex.DumpDebug();

                this.AuthenticationFailed?.Invoke(this, new()
                {
                    Instruction = "認証失敗",
                    Message = "認証処理が失敗しました。\nしばらく時間をおいてから再度お試しください。",
                });
                this.Cancelled?.Invoke(this, new());
                return;
            }

            this.Account = this._accounts.RegisterOrUpdate(this._api, account);
            _ = this.Account.StartActivity();

            this.PageIndex = PageIndices.Complete;

            this.IsBusy = false;
        }

        /// <summary>
        /// 次のページに遷移可能か取得する
        /// </summary>
        /// <returns></returns>
        private bool CanNavigateToNextPage()
        {
            if (this.IsBusy)
            {
                return false;
            }

            if (this.PageIndex == PageIndices.PinCode)
            {
                return !string.IsNullOrWhiteSpace(this.PinCode);
            }

            return this.PageIndex != PageIndices.Complete;
        }

        private Command _cancelCommand;
        /// <summary>
        /// キャンセルコマンド
        /// </summary>
        public Command CancelCommand => this._cancelCommand ??= new DelegateCommand(
            () =>
            {
                if (this.PageIndex == PageIndices.Complete)
                {
                    this.Completed?.Invoke(this, new());
                }
                else
                {
                    this.Cancelled?.Invoke(this, new());
                }
            },
            () => !this.IsBusy);
    }
}
