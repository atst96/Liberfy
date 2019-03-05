using Liberfy.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Liberfy.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public WindowStatus WindowStatus { get; }

        public MainWindowViewModel() : base()
        {
            this.Accounts = AccountManager.Accounts;
            this.SelectedAccount = this.Accounts.FirstOrDefault();
            this.WindowStatus = App.Setting.Window.Main;
        }

        public IEnumerable<IAccount> Accounts { get; }

        private IAccount _selectedAccount;
        public IAccount SelectedAccount
        {
            get => this._selectedAccount;
            set => this.SetProperty(ref this._selectedAccount, value);
        }

        private bool _initialized;
        internal override void OnInitialized()
        {
            if (this._initialized) return;
            this._initialized = true;
        }

        private Command _showSettingDialog;
        public Command ShowSettingDialog
        {
            get => this._showSettingDialog ?? (this._showSettingDialog = this.RegisterCommand(() => this.WindowService.OpenSetting()));
        }

        private Command<IAccount> _openTweetWindowCommand;
        public Command<IAccount> OpenTweetWindowCommand
        {
            get => this._openTweetWindowCommand ?? (this._openTweetWindowCommand = this.RegisterCommand(new OpenTweetWindowCommand(this)));
        }

        public IEnumerable<ColumnBase> Columns { get; } = TimelineBase.Columns;
    }
}
