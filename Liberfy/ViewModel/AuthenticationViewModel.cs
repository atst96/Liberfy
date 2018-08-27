using SocialApis;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using MastodonTokens = SocialApis.Mastodon.Tokens;

namespace Liberfy.ViewModel
{
    internal class AuthenticationViewModel : ContentWindowViewModel
    {
        public AuthenticationViewModel() : base()
        {
            this.Title = "認証";
            this.SelectedService = SocialService.Twitter;
        }

        public IReadOnlyDictionary<SocialService, string> ServiceNameList { get; } = new ReadOnlyDictionary<SocialService, string>(
            new Dictionary<SocialService, string>
            {
                [SocialService.Twitter] = "Twitter",
                [SocialService.Mastodon] = "Mastodon",
            });

        public static IReadOnlyDictionary<SocialService, ISocialConfig> ServiceConfigList { get; } = new ReadOnlyDictionary<SocialService, ISocialConfig>(
            new Dictionary<SocialService, ISocialConfig>
            {
                [SocialService.Twitter] = new TwitterConfig(),
                [SocialService.Mastodon] = new MastodonConfig(),
            });

        private SocialService _selectedService;
        public SocialService SelectedService
        {
            get => this._selectedService;
            set
            {
                if (this.SetProperty(ref this._selectedService, value))
                {
                    this.ServiceConfig = ServiceConfigList[value];
                    this.RaisePropertyChanged(nameof(this.ServiceConfig));

                    this._nextCommand?.RaiseCanExecute();
                }
            }
        }

        public ISocialConfig ServiceConfig { get; private set; }

        private bool _overrideKey;
        public bool OverrideKey
        {
            get => this._overrideKey;
            set => SetProperty(ref this._overrideKey, value, this._nextCommand);
        }

        private string _consumerKey;
        public string ConsumerKey
        {
            get => this._consumerKey;
            set => this.SetProperty(ref this._consumerKey, value, this._nextCommand);
        }

        private string _consumerSecret;
        public string ConsumerSecret
        {
            get => this._consumerSecret;
            set => this.SetProperty(ref this._consumerSecret, value, this._nextCommand);
        }

        private string _instanceUrl;
        public string InstanceUrl
        {
            get => this._instanceUrl;
            set => this.SetProperty(ref this._instanceUrl, value, this._nextCommand);
        }

        private string _error;
        public string Error
        {
            get => this._error;
            set
            {
                if (this.SetProperty(ref this._error, value))
                {
                    this.HasError = !string.IsNullOrEmpty(value);
                    this.RaisePropertyChanged(nameof(this.HasError));
                }
            }
        }

        public bool HasError { get; private set; } = false;

        private string _pinCode;
        public string PinCode
        {
            get => _pinCode;
            set => SetProperty(ref _pinCode, value, _nextCommand);
        }

        // page-0: CK/CSの入力
        // page-1: PINコード入力
        // page-2: 
        private int _pageIndex;
        public int PageIndex
        {
            get { return _pageIndex; }
            set { SetProperty(ref _pageIndex, value, _nextCommand); }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { SetProperty(ref _isRunning, value, _nextCommand); }
        }

        public RequestTokenResponse Session { get; private set; }

        public Tokens TwitterTokens { get; private set; }

        public MastodonTokens MastodonTokens { get; private set; }

        private CancellationTokenSource _tokenSource;

        #region Command: NextCommand

        private Command _nextCommand;
        public Command NextCommand
        {
            get => _nextCommand ?? (_nextCommand = this.RegisterCommand(this.MoveNextPage, this.CanMoveNextPage));
        }

        private async void MoveNextPage()
        {
            this.Error = string.Empty;

            if (_pageIndex == 0)
            {
                // page-0: 認証URLの取得

                string cKey = null;
                string cSec = null;

                if (this.OverrideKey)
                {
                    (cKey, cSec) = (this.ConsumerKey, this.ConsumerSecret);
                }

                if (this.SelectedService == SocialService.Twitter)
                {
                    if (!this.OverrideKey)
                    {
                        (cKey, cSec) = (Config.Twitter.ConsumerKey, Config.Twitter.ConsumerSecret);
                    }

                    try
                    {
                        this._tokenSource = new CancellationTokenSource();

                        this.IsRunning = true;

                        this.TwitterTokens = new Tokens(cKey, cSec);
                        this.Session = await TwitterTokens.OAuth.RequestToken();

                        App.Open(this.Session.GetAuthorizeUrl());
                        this.PageIndex++;
                    }
                    catch (Exception ex)
                    {
                        this.Error = ex.Message;
                    }
                    finally
                    {
                        this.IsRunning = false;
                    }
                }
                else if (this.SelectedService == SocialService.Mastodon)
                {
                    this._tokenSource = new CancellationTokenSource();

                    this.IsRunning = true;

                    try
                    {
                        var instanceUrl = this.InstanceUrl;

                        if (Uri.CheckSchemeName(instanceUrl))
                        {
                            instanceUrl = "https://" + instanceUrl;
                        }

                        if (!Uri.TryCreate(instanceUrl, UriKind.Absolute, out var uri))
                        {
                            this.DialogService.MessageBox(
                                "インスタンスの正しいURLを入力してください。",
                                 MsgBoxButtons.Ok,
                                 MsgBoxIcon.Error);
                            return;
                        }

                        var apiScopes = new[] { "read", "write", "follow" };

                        if (!this.OverrideKey)
                        {
                            string uri_url = uri.ToString();

                            var key = App.Setting.ClientKeys
                                .Where(k => k.Service == SocialService.Mastodon)
                                .FirstOrDefault(k => k.Host == uri_url);

                            if (key == null)
                            {
                                var clientKey = await MastodonTokens.Apps.Register(uri, App.AppName, apiScopes);
                                this.MastodonTokens = new MastodonTokens(uri, clientKey);

                                App.Setting.ClientKeys.Add(new ClientKeyCache(uri, clientKey));
                            }
                            else
                            {
                                this.MastodonTokens = new MastodonTokens(uri, key.ClientId, key.ClientSecret);
                            }
                        }
                        else
                        {
                            this.MastodonTokens = new MastodonTokens(uri, this.ConsumerKey, this.ConsumerSecret);
                        }

                        var url = this.MastodonTokens.OAuth.GetAuthorizeUrl(apiScopes);
                        App.Open(url);

                        this.PageIndex++;
                    }
                    catch (Exception ex)
                    {
                        this.Error = ex.Message;
                    }
                    finally
                    {
                        this.IsRunning = false;
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else if (_pageIndex == 1)
            {
                // page-1: PINコードを用いて認証

                if (this.SelectedService == SocialService.Twitter)
                {
                    try
                    {
                        this._tokenSource = new CancellationTokenSource();

                        this.IsRunning = true;

                        this.TwitterTokens = await TwitterTokens.OAuth.AccessToken(this.Session, this._pinCode);

                        this.DialogService.Close(true);
                    }
                    catch (Exception ex)
                    {
                        this.Error = ex.Message;
                        this.PageIndex++;
                        this.IsRunning = false;
                    }
                }
                else if (this.SelectedService == SocialService.Mastodon)
                {
                    try
                    {
                        this._tokenSource = new CancellationTokenSource();

                        this.IsRunning = true;

                        (var host, var cId, var cSec) = (MastodonTokens.HostUrl, MastodonTokens.ClientId, MastodonTokens.ClientSecret);

                        var res = await MastodonTokens.OAuth.GetAccessToken(this.PinCode);

                        this.MastodonTokens = new MastodonTokens(host, cId, cSec, res.AccessToken);

                        this.DialogService.Close(true);
                    }
                    catch (Exception ex)
                    {
                        this.Error = ex.Message;
                        this.PageIndex++;
                        this.IsRunning = false;
                    }
                }
                else
                {

                    throw new NotImplementedException();
                }
            }
        }

        private bool CanMoveNextPage()
        {
            if (IsRunning)
                return false;

            switch (_pageIndex)
            {
                case 0:
                    if (_overrideKey)
                    {
                        return
                            this.ServiceNameList.ContainsKey(this.SelectedService)
                            && !string.IsNullOrWhiteSpace(this.ConsumerKey)
                            && !string.IsNullOrWhiteSpace(this.ConsumerSecret)
                            && (!this.ServiceConfig.IsVariableDomain || !string.IsNullOrEmpty(this.InstanceUrl));
                    }
                    else
                    {
                        return true;
                    }

                case 1:
                    return !string.IsNullOrEmpty(this.PinCode);

                default:
                    return false;
            }
        }

        #endregion

        #region Command: CancelCommand

        private Command _cancelCommand;
        public Command CancelCommand
        {
            get => _cancelCommand ?? (_cancelCommand = RegisterCommand(CancelAll));
        }

        private void CancelAll()
        {
            if (_tokenSource != null
                && !_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
            }

            DialogService.Close(false);
        }

        #endregion

        #region Command: CopyClipboardCommand

        private Command _copyClipboardCommand;
        public Command CopyClipboardCommand => this._copyClipboardCommand ?? (this._copyClipboardCommand = this.RegisterCommand(() =>
        {
            if (this.Session != null)
            {
                System.Windows.Clipboard.SetText(this.Session.GetAuthorizeUrl());
            }
        }));

        #endregion
    }
}
