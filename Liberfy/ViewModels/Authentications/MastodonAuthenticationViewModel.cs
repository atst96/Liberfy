using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Liberfy.Data.InstanceKeys;
using Liberfy.Data.Mastodon;
using Liberfy.Managers;
using Liberfy.Utils;
using SocialApis.Mastodon;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels.Authentications
{
    /// <summary>
    /// <see cref="Views.Authentications.MastodonAuthenticationView"/>のViewModel
    /// </summary>
    internal sealed class MastodonAuthenticationViewModel : NotificationObject
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
            /// 認証コード入力ページ
            /// </summary>
            public const int AuthCode = 1;

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
        /// API
        /// </summary>
        private MastodonApi _api;

        private MastodonAccount _account;

        /// <summary>
        /// アカウント情報
        /// </summary>
        public MastodonAccount Account
        {
            get => this._account;
            private set => this.SetProperty(ref this._account, ref value);
        }

        /// <summary>
        /// インスタンス名の正規表現
        /// </summary>
        private readonly Regex _instanceRegex = new("^[a-zA-Z0-9][a-zA-Z0-9\\-\\.]+[a-zA-Z]+$", RegexOptions.Compiled);

        private bool _isInstanceLoading;

        /// <summary>
        /// インスタンス情報の読み込み中フラグ
        /// </summary>
        public bool IsInstanceLoading
        {
            get => this._isInstanceLoading;
            private set => this.SetProperty(ref this._isInstanceLoading, value);
        }

        private string _loadInstanceErrorMessage;

        /// <summary>
        /// インスタンス読み込みエラー
        /// </summary>
        public string LoadInstanceErrorMessage
        {
            get => this._loadInstanceErrorMessage;
            private set => this.SetProperty(ref this._loadInstanceErrorMessage, value);
        }

        private bool _isLoadInstanceFailed;

        /// <summary>
        /// インスタンスの読み込みが失敗のフラグ
        /// </summary>
        public bool IsLoadInstanceFailed
        {
            get => this._isLoadInstanceFailed;
            private set => this.SetProperty(ref this._isLoadInstanceFailed, value);
        }

        private string _instanceName;

        /// <summary>
        /// インスタンス
        /// </summary>
        public string InstanceName
        {
            get => this._instanceName;
            set
            {
                if (!this.SetProperty(ref this._instanceName, value))
                {
                    return;
                }

                this.UpdateInstanceInfo(value);
            }
        }

        /// <summary>
        /// インスタンス情報取得時のエラーを設定する
        /// </summary>
        /// <param name="message"></param>
        private void SetInstanceError(string message)
        {
            this.LoadInstanceErrorMessage = message;
            this.IsLoadInstanceFailed = true;
        }

        /// <summary>
        /// インスタンス情報のエラーをクリアする
        /// </summary>
        private void ClearInstanceError()
        {
            this.LoadInstanceErrorMessage = default;
            this.IsLoadInstanceFailed = false;
        }

        private void UpdateIsInvalid()
        {
            this.NextCommand.RaiseCanExecute();
        }

        /// <summary>
        /// インスタンス情報を更新する
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        private async void UpdateInstanceInfo(string instanceName)
        {
            this.IsInstanceLoading = true;
            this.Instance = default;
            this.ClearInstanceError();

            try
            {
                if (string.IsNullOrEmpty(instanceName))
                {
                    this.SetInstanceError("インスタンス名を入力してください");
                    return;
                }

                if (!this._instanceRegex.IsMatch(instanceName))
                {
                    this.SetInstanceError("無効なインスタンス名です");
                    return;
                }

                if (!Uri.TryCreate($"https://{instanceName}", UriKind.Absolute, out var url))
                {
                    this.SetInstanceError("無効なインスタンス名です");
                    return;
                }

                var api = new MastodonApi(url, null, null);

                Instance instance;
                MastodonKeyInfo keyInfo;
                try
                {
                    instance = await api.Instances.GetInstance();
                    keyInfo = await ClientKeyManager.GetMastodonKey(this.InstanceName);
                }
                catch (HttpRequestException hrex)
                {
                    this.SetInstanceError("インスタンスへの接続に失敗しました");
                    return;
                }

                // Faviconを取得する
                var favicon = await WebPageUtil.GetFaviconUrl(url);

                this._instanceKeyInfo = keyInfo;
                this._api = new MastodonApi(url, keyInfo.ClientId, keyInfo.ClientSecret);
                this.Instance = new InstanceInfo
                {
                    Title = instance.Title,
                    Description = instance.Description,
                    Icon = favicon,
                    Url = url,
                    StreamingApiEndpoint = instance.Urls?.StreamingApi,
                    Version = instance.Version,
                    Languages = instance.Languages,
                };

                App.MastodonInstances.TryAdd(this.InstanceName, this.Instance);
            }
            finally
            {
                this.IsInstanceLoading = false;
                this.UpdateIsInvalid();
            }
        }

        private InstanceInfo _instance;
        private MastodonKeyInfo _instanceKeyInfo;

        /// <summary>
        /// インスタンス情報
        /// </summary>
        public InstanceInfo Instance
        {
            get => this._instance;
            private set
            {
                if (this.SetProperty(ref this._instance, value))
                {
                    this.RaisePropertyChanged(nameof(this.HasInstance));
                }
            }
        }

        /// <summary>
        /// インスタンス情報の有無
        /// </summary>
        public bool HasInstance => this.Instance != null;

        /// <summary>
        /// <see cref="MastodonAuthenticationViewModel"/>を生成する
        /// </summary>
        public MastodonAuthenticationViewModel() : base()
        {
            this._settings = App.Setting;
            this._accounts = App.Accounts;
        }

        private readonly PropertyChangedEventArgs _pinCodeProperty = new(nameof(AuthCode));
        private string _authCode;
        /// <summary>
        /// 認証コード
        /// </summary>
        public string AuthCode
        {
            get => this._authCode;
            set
            {
                if (this.SetProperty(ref this._authCode, ref value, this._pinCodeProperty))
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
            else if (this.PageIndex == PageIndices.AuthCode)
            {
                return this.Authorize();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 認証画面のURLを取得し、表示する
        /// </summary>
        /// <returns></returns>
        private Task GetAuthorizeUrl()
        {
            this.IsBusy = true;

            string[] scopes = { "read", "write", "follow" };
            this.AuthorizeUrl = this._api.OAuth.GetAuthorizeUrl(scopes, "urn:ietf:wg:oauth:2.0:oob");

            App.Open(this.AuthorizeUrl);

            this.PageIndex = PageIndices.AuthCode;

            this.IsBusy = false;

            return Task.CompletedTask;
        }

        /// <summary>
        /// 入力された認証コードを用いてアカウントの認証を行う
        /// </summary>
        /// <returns></returns>
        private async Task Authorize()
        {
            var authCode = this.AuthCode.Trim();

            this.IsBusy = true;

            AccessTokenResponse tokens;
            try
            {
                tokens = await this._api.OAuth.GetAccessToken(authCode);
            }
            catch (Exception ex)
            {
                // TODO: ログ
                ex.DumpDebug();

                this.AuthenticationFailed?.Invoke(this, new AuthenticationFailedMessage
                {
                    Instruction = "認証失敗",
                    Message = "認証処理が失敗しました。\nしばらく時間をおいてから再度お試しください。",
                });
                this.Cancelled?.Invoke(this, new());
                return;
            }

            this._api = new(this._api.HostUrl, this._api.ClientId, this._api.ClientSecret, tokens.AccessToken);

            Account account;
            try
            {
                account = await this._api.Accounts.VerifyCredentials();
            }
            catch (Exception ex)
            {
                // TODO: ログ
                ex.DumpDebug();

                this.AuthenticationFailed?.Invoke(this, new AuthenticationFailedMessage
                {
                    Instruction = "認証失敗",
                    Message = "認証処理が失敗しました。\nしばらく時間をおいてから再度お試しください。",
                });
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

            return this.PageIndex switch
            {
                PageIndices.Top => this.Instance != null,
                PageIndices.AuthCode => !string.IsNullOrWhiteSpace(this.AuthCode),
                _ => this.PageIndex != PageIndices.Complete,
            };
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
