using Liberfy.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Commands.AccountAuthenticationWindowCommands
{
    internal class NextCommand : Command
    {
        private AccountAuthenticationWindowViewModel _viewModel;

        private bool IsRunning
        {
            get => this._viewModel.IsRunning;
            set => this._viewModel.IsRunning = value;
        }

        private int AuthenticationPhase
        {
            get => this._viewModel.AuthenticationPhase;
            set => this._viewModel.AuthenticationPhase = value;
        }

        private string Error
        {
            get => this._viewModel.Error;
            set => this._viewModel.Error = value;
        }

        private string ConsumerKey => this._viewModel.ConsumerKey;

        private string ConsumerSecret => this._viewModel.ConsumerSecret;

        private string InstanceUrl => this._viewModel.InstanceUrl;

        private string VerificationCode => this._viewModel.VerificationCode;

        private ServiceType SelectedService => this._viewModel.SelectedService;

        private ISocialConfig ServiceConfig => this._viewModel.ServiceConfig;

        private IAccountAuthenticator AccountAuthenticator => this._viewModel.AccountAuthenticator;

        private bool OverrideKey => this._viewModel.OverrideKey;

        private IReadOnlyDictionary<ServiceType, string> ServiceNames { get; }

        private IReadOnlyDictionary<ServiceType, ISocialConfig> ServiceConfigs { get; }

        public NextCommand(AccountAuthenticationWindowViewModel viewModel)
        {
            this._viewModel = viewModel;
            this.ServiceNames = this._viewModel.ServiceNames;
            this.ServiceConfigs = this._viewModel.ServiceConfigs;
        }

        protected override bool CanExecute(object parameter)
        {
            if (this.IsRunning)
            {
                return false;
            }

            if (this.AuthenticationPhase == AccountAuthenticationWindowViewModel.AuthenticationPhases.SelectService)
            {
                bool isValidService = this.ServiceNames.ContainsKey(this.SelectedService);
                bool isValidDomain = !this.ServiceConfig.IsVariableDomain || !string.IsNullOrWhiteSpace(this.InstanceUrl);

                return isValidService && isValidDomain && (!this.OverrideKey || (!string.IsNullOrWhiteSpace(this.ConsumerKey) && !string.IsNullOrWhiteSpace(this.ConsumerSecret)));
            }
            else if (this.AuthenticationPhase == AccountAuthenticationWindowViewModel.AuthenticationPhases.InputVerificationCode)
            {
                return !string.IsNullOrEmpty(this.VerificationCode);
            }

            return false;
        }

        protected async override void Execute(object parameter)
        {
            this.Error = string.Empty;

            if (this.AuthenticationPhase == AccountAuthenticationWindowViewModel.AuthenticationPhases.SelectService)
            {
                // page-0: 認証URLの取得

                string consumerKey = null;
                string consumerSecret = null;
                Uri instanceUri = null;

                if (!string.IsNullOrEmpty(this.InstanceUrl))
                {
                    try
                    {
                        var url = this.InstanceUrl;

                        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                        {
                            url = "https://" + url;
                        }

                        instanceUri = new Uri(url, UriKind.Absolute);
                    }
                    catch
                    {
                        this.Error = "URLを正しく入力してください";
                        return;
                    }
                }

                if (this.OverrideKey)
                {
                    (consumerKey, consumerSecret) = (this.ConsumerKey, this.ConsumerSecret);
                }

                try
                {
                    this.IsRunning = true;

                    await this.AccountAuthenticator.Authentication(instanceUri, consumerKey, consumerSecret);

                    this.AuthenticationPhase = AccountAuthenticationWindowViewModel.AuthenticationPhases.InputVerificationCode;
                }
                catch (Exception ex)
                {
                    this._viewModel.DialogService.MessageBox(ex.GetMessage(), "Error");
                }

                this.IsRunning = false;
            }
            else if (this.AuthenticationPhase == AccountAuthenticationWindowViewModel.AuthenticationPhases.InputVerificationCode)
            {
                // page-1: PINコードを用いて認証

                this.IsRunning = true;

                try
                {
                    await this.AccountAuthenticator.GetAccessToken(this.VerificationCode);
                    await AccountManager.Register(this.AccountAuthenticator);
                    this._viewModel.DialogService.Close(false);
                }
                catch (Exception ex)
                {
                    this.Error = ex.GetMessage();
                    this.AuthenticationPhase = AccountAuthenticationWindowViewModel.AuthenticationPhases.Error;
                }

                this.IsRunning = false;
            }
        }
    }
}
