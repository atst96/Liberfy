using Liberfy.Commands.AccountAuthenticationWindowCommands;
using Liberfy.Services.Mastodon;
using Liberfy.Services.Twitter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels
{
    internal class AccountAuthenticationWindowViewModel : ViewModelBase
    {
        internal static class AuthenticationPhases
        {
            public const int SelectService = 0;
            public const int InputVerificationCode = 1;
            public const int Error = 2;
        }

        public AccountAuthenticationWindowViewModel() : base()
        {
            this.ServiceNames = new Dictionary<ServiceType, string>
            {
                [ServiceType.Twitter] = "Twitter",
                [ServiceType.Mastodon] = "Mastodon",
            };

            this.ServiceConfigs = new Dictionary<ServiceType, ISocialConfig>
            {
                [ServiceType.Twitter] = new TwitterConfig(),
                [ServiceType.Mastodon] = new MastodonConfig(),
            };

            this.OnSelectedServiceUpdated(this.SelectedService);
        }

        public IReadOnlyDictionary<ServiceType, string> ServiceNames { get; }

        public IReadOnlyDictionary<ServiceType, ISocialConfig> ServiceConfigs { get; }

        public static readonly IReadOnlyDictionary<ServiceType, Type> _authenticatorTypes = new Dictionary<ServiceType, Type>
        {
            [ServiceType.Twitter ] = typeof(TwitterAccountAuthenticator ),
            [ServiceType.Mastodon] = typeof(MastodonAccountAuthenticator),
        };

        private IList<string> _cachedInstanceUrls;
        public IList<string> CachedInstanceUrls
        {
            get => this._cachedInstanceUrls;
            private set => this.SetProperty(ref this._cachedInstanceUrls, value);
        }

        private ISocialConfig _serviceConfig;
        public ISocialConfig ServiceConfig
        {
            get => this._serviceConfig;
            private set => this.SetProperty(ref this._serviceConfig, value);
        }

        private ServiceType _selectedService = ServiceType.Twitter;
        public ServiceType SelectedService
        {
            get => this._selectedService;
            set
            {
                if (this.SetProperty(ref this._selectedService, value))
                {
                    this.OnSelectedServiceUpdated(value);
                }
            }
        }

        private void OnSelectedServiceUpdated(ServiceType type)
        {
            this.ServiceConfig = this.ServiceConfigs[type];
            this.AccountAuthenticator = (IAccountAuthenticator)Activator.CreateInstance(_authenticatorTypes[type]);
            this.CachedInstanceUrls = App.Setting.ClientKeys.Where(key => key.Service == type).Select(key => key.Host).ToList();
        }

        private IAccountAuthenticator _accountAuthenticator;
        public IAccountAuthenticator AccountAuthenticator
        {
            get => this._accountAuthenticator;
            private set
            {
                if (this.AuthenticationPhase != AuthenticationPhases.SelectService)
                {
                    throw new InvalidOperationException();
                }

                this.SetProperty(ref this._accountAuthenticator, value);
            }
        }

        private bool _overrideKey;
        public bool OverrideKey
        {
            get => this._overrideKey;
            set
            {
                if (this.SetProperty(ref this._overrideKey, value))
                {
                    this._nextCommand.RaiseCanExecute();
                }
            }
        }

        private string _consumerKey;
        public string ConsumerKey
        {
            get => this._consumerKey;
            set
            {
                if (this.SetProperty(ref this._consumerKey, value))
                {
                    this._nextCommand.RaiseCanExecute();
                }
            }
        }

        private string _consumerSecret;
        public string ConsumerSecret
        {
            get => this._consumerSecret;
            set
            {
                if (this.SetProperty(ref this._consumerSecret, value))
                {
                    this._nextCommand.RaiseCanExecute();
                }
            }
        }

        private string _instanceUrl;
        public string InstanceUrl
        {
            get => this._instanceUrl;
            set
            {
                if (this.SetProperty(ref this._instanceUrl, value))
                {
                    this._nextCommand.RaiseCanExecute();
                }
            }
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

        private string _verificationCode;
        public string VerificationCode
        {
            get => this._verificationCode;
            set
            {
                if (this.SetProperty(ref this._verificationCode, value))
                {
                    this._nextCommand.RaiseCanExecute();
                }
            }
        }

        private int _authenticationPhase = AuthenticationPhases.SelectService;
        public int AuthenticationPhase
        {
            get => this._authenticationPhase;
            set
            {
                if (this.SetProperty(ref this._authenticationPhase, value))
                {
                    this._nextCommand.RaiseCanExecute();
                }
            }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get => this._isRunning;
            set
            {
                if (this.SetProperty(ref this._isRunning, value))
                {
                    this._nextCommand.RaiseCanExecute();
                }
            }
        }

        private Command _nextCommand;
        public Command NextCommand => this._nextCommand ?? (this._nextCommand = this.RegisterCommand(new NextCommand(this)));

        private Command _cancelCommand;
        public Command CancelCommand => this._cancelCommand ?? (this._cancelCommand = this.RegisterCommand(new CancelCommand(this)));

        private Command _copyClipboardCommand;
        public Command CopyClipboardCommand => this._copyClipboardCommand ?? (this._copyClipboardCommand = this.RegisterCommand(new CopyClipboardCommand(this)));
    }
}
