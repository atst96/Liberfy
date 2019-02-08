using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using Liberfy.Settings;
using SocialApis;
using Liberfy.Commands.SettingWindowCommands;

namespace Liberfy.ViewModel
{
    internal partial class SettingWindowViewModel : ViewModelBase
    {
        public SettingWindowViewModel() : base()
        {
            this.DefaultColumns = new NotifiableCollection<ColumnBase>(Setting.DefaultColumns
                .Select(opt => ColumnBase.FromSetting(opt, null, out var column) ? column : ColumnBase.FromType(opt.Type)));
        }

        public static Setting GlobalSetting { get; } = App.Setting;

        public Setting Setting { get; } = GlobalSetting;

        public IEnumerable<IAccount> Accounts { get; } = AccountManager.Accounts;

        /*
         * [表示]タブ関連 → SettingWindowViewModel.View.cs
         * 
         * 
         */

        private const string AutoStartupRegSubKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

        private RegistryKey CreateAutoStartuRegSubKey()
        {
            return Registry.CurrentUser.OpenSubKey(AutoStartupRegSubKey, true);
        }

        private bool GetAutoStartupEnabled()
        {
            using (var reg = CreateAutoStartuRegSubKey())
            {
                var path = Regex.Escape(App.Assembly.Location);

                return reg.GetValue(App.AppName) is string regKeyValue
                    && !string.IsNullOrEmpty(regKeyValue)
                    && Regex.IsMatch(regKeyValue, $@"^(""{path}""|{path})(?<params>\s+.*)?$", RegexOptions.IgnoreCase);
            }
        }

        public bool IsAccountPage { get; private set; }

        private int _tabPageIndex;
        public int TabPageIndex
        {
            get => _tabPageIndex;
            set
            {
                if (SetProperty(ref _tabPageIndex, value))
                {
                    IsAccountPage = value == 1;
                    RaisePropertyChanged(nameof(IsAccountPage));
                }
            }
        }

        public bool ShowInTaskTray
        {
            get => Setting.ShowInTaskTray;
            set
            {
                Setting.ShowInTaskTray = value;
                RaisePropertyChanged(nameof(ShowInTaskTray));
            }
        }

        public bool AutoStartup
        {
            get => GetAutoStartupEnabled();
            set
            {
                try
                {
                    bool isAutoStartupEnabled = GetAutoStartupEnabled();
                    if (Equals(isAutoStartupEnabled, value)) return;

                    using (var reg = CreateAutoStartuRegSubKey())
                    {
                        if (isAutoStartupEnabled)
                        {
                            reg.DeleteValue(App.AppName);
                        }
                        else
                        {
                            reg.SetValue(App.AppName, $"\"{App.Assembly.Location}\" /startup");
                        }
                    }

                    DialogService.MessageBox($"自動起動を{(value ? "有効" : "無効")}にしました", MsgBoxButtons.Ok, MsgBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    DialogService.MessageBox("自動起動の設定に失敗しました: " + ex.Message, MsgBoxButtons.Ok, MsgBoxIcon.Information);
                }

                RaisePropertyChanged(nameof(AutoStartup));
            }
        }

        public bool ShowInTaskTrayAtMinimized
        {
            get => Setting.ShowInTaskTrayAtMinimzied;
            set
            {
                Setting.ShowInTaskTrayAtMinimzied = value;
                RaisePropertyChanged(nameof(ShowInTaskTrayAtMinimized));
            }
        }

        #region General

        public IReadOnlyDictionary<TextFormattingMode, string> TextRenderingModeList { get; } = new ReadOnlyDictionary<TextFormattingMode, string>(new Dictionary<TextFormattingMode, string>
        {
            [TextFormattingMode.Ideal] = "標準",
            [TextFormattingMode.Display] = "GDI互換",
        });

        private FontFamily _timelineViewFontFamily = new FontFamily(string.Join(",", GlobalSetting.TimelineFont));
        public FontFamily TimelineViewFontFamily
        {
            get => this._timelineViewFontFamily;
            set => this.SetProperty(ref this._timelineViewFontFamily, value);
        }

        private double? _timelineFontSize = GlobalSetting.TimelineFontSize;
        public double? TimelineFontSize
        {
            get => this._timelineFontSize;
            set => this.SetProperty(ref this._timelineFontSize, value);
        }

        private string _timelineFontFamilies = string.Join("\n", GlobalSetting.TimelineFont);
        public string TimelineFontFamily
        {
            get => this._timelineFontFamilies;
            set
            {
                if (this.SetProperty(ref this._timelineFontFamilies, value))
                {
                    var fonts = EnumerateFontName(value);

                    this.TimelineViewFontFamily = new FontFamily(string.Join(",", fonts));
                }
            }
        }

        private TextFormattingMode _timelineFontRenderingMode = GlobalSetting.TimelineFontRendering;
        public TextFormattingMode TimelineFontRenderingMode
        {
            get => this._timelineFontRenderingMode;
            set => this.SetProperty(ref this._timelineFontRenderingMode, value);
        }

        private static IEnumerable<string> EnumerateFontName(string text)
        {
            return text
                .Split('\r', '\n', ',')
                .Select(str => str.Trim())
                .Where(str => str.Length > 1);
        }

        public FontFamily _selectedFontFamily;
        public FontFamily SelectedFontFamily
        {
            get => this._selectedFontFamily;
            set
            {
                if (this.SetProperty(ref this._selectedFontFamily, value))
                {
                    if (value != null)
                    {
                        if (this.TimelineFontFamily.EndsWith("\n"))
                        {
                            this.TimelineFontFamily += value.Source;
                        }
                        else
                        {
                            this.TimelineFontFamily += "\n" + value.Source;
                        }

                        this.SelectedFontFamily = null;
                    }
                }
            }
        }

        #endregion

        #region Accounts

        private IAccount _selectedAccount;
        public IAccount SelectedAccount
        {
            get => this._selectedAccount;
            set
            {
                if (this.SetProperty(ref this._selectedAccount, value))
                {
                    this.RaiseCanExecuteAccountCommands();
                }
            }
        }

        #region Commands for account

        #region Command: AccountAddCommand

        private Command _accountAddCommand;
        public Command AccountAddCommand => this._accountAddCommand ?? (this._accountAddCommand = this.RegisterCommand(this.AccountAdd));

        private async void AccountAdd()
        {
            var auth = new AuthenticationViewModel();

            if (DialogService.OpenModal(auth, new ViewOption
            {
                ResizeMode = ResizeMode.NoResize,
                StartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.Manual,
                Width = 400,
                Height = 320,
                WindowChrome = new System.Windows.Shell.WindowChrome
                {
                    GlassFrameThickness = new Thickness(0),
                    UseAeroCaptionButtons = false,
                    CornerRadius = new CornerRadius(0),
                    CaptionHeight = 0,
                },
                ShowInTaskbar = false,
            }, typeof(AuthenticationViewModel)))
            {
                if (auth.SelectedService == SocialService.Twitter)
                {
                    var tokens = auth.TwitterTokens;

                    var account = AccountManager.Get(ServiceType.Twitter, tokens.UserId);

                    if (account != null)
                    {
                        account.SetClient(ApiTokenInfo.FromTokens(tokens));
                    }
                    else
                    {
                        var user = await tokens.Account.VerifyCredentials();

                        account = new TwitterAccount(tokens, user);

                        AccountManager.Add(account);

                        foreach (var columnOptions in this.DefaultColumns.Select(c => c.GetOption()))
                        {
                            if (ColumnBase.FromSetting(columnOptions, account, out var column))
                            {
                                TimelineBase.Columns.Add(column);
                            }
                        }
                    }

                    if (!await account.Login())
                    {
                        this.DialogService.MessageBox(
                            $"アカウント情報の取得に失敗しました:\n",
                            MsgBoxButtons.Ok, MsgBoxIcon.Information);

                        AccountManager.Remove(account);
                    }
                }
                else if (auth.SelectedService == SocialService.Mastodon)
                {
                    var tokens = auth.MastodonTokens;

                    var user = await tokens.Accounts.VerifyCredentials();


                    var account = AccountManager.Get(ServiceType.Mastodon, user.Id);

                    if (account != null)
                    {
                        account.SetClient(ApiTokenInfo.FromTokens(tokens));
                    }
                    else
                    {
                        account = new MastodonAccount(tokens, user);

                        AccountManager.Add(account);

                        foreach (var columnOptions in this.DefaultColumns.Select(c => c.GetOption()))
                        {
                            if (ColumnBase.FromSetting(columnOptions, account, out var column))
                            {
                                TimelineBase.Columns.Add(column);
                            }
                        }
                    }

                    if (!await account.Login())
                    {
                        this.DialogService.MessageBox(
                            $"アカウント情報の取得に失敗しました:\n",
                            MsgBoxButtons.Ok, MsgBoxIcon.Information);

                        AccountManager.Remove(account);
                    }
                }
            }
        }

        #endregion

        internal void RaiseCanExecuteAccountCommands()
        {
            this._accountMoveUpCommand?.RaiseCanExecute();
            this._accountMoveDownCommand?.RaiseCanExecute();
            this._accountDeleteCommand?.RaiseCanExecute();
        }

        private Command<IAccount> _accountDeleteCommand;
        public Command<IAccount> AccountDeleteCommand => this._accountDeleteCommand ?? (this._accountDeleteCommand = this.RegisterCommand(new AccountDeleteCommand(this)));

        private Command<IAccount> _accountMoveUpCommand;
        public Command<IAccount> AccountMoveUpCommand => this._accountMoveUpCommand ?? (this._accountMoveUpCommand = this.RegisterCommand(new AccountMoveUpCommand(this)));

        private Command<IAccount> _accountMoveDownCommand;
        public Command<IAccount> AccountMoveDownCommand => this._accountMoveDownCommand = this._accountMoveDownCommand = this.RegisterCommand(new AccountMoveDownCommand(this));

        #endregion Commands for account


        public NotifiableCollection<ColumnBase> DefaultColumns { get; }

        #region Commands for columns

        private ColumnBase _selectedColumnSetting;
        public ColumnBase SelectedColumnSetting
        {
            get => this._selectedColumnSetting;
            set
            {
                if (this.SetProperty(ref _selectedColumnSetting, value))
                {
                    this.ColumnMoveUpCommand.RaiseCanExecute();
                    this.ColumnMoveDownCommand.RaiseCanExecute();
                    this.ColumnRemoveCommand.RaiseCanExecute();
                }
            }
        }

        #region Command: ColumnAddCommand

        private Command<ColumnType> _columnAddCommand;
        public Command<ColumnType> ColumnAddCommand => this._columnAddCommand ?? (this._columnAddCommand = this.RegisterCommand((ColumnType key) =>
        {
            this.DefaultColumns.Add(ColumnBase.FromType(key));
        }));

        #endregion

        #region Command: ColumnRemoveCommand

        private Command<ColumnBase> _columnRemoveCommand;
        public Command<ColumnBase> ColumnRemoveCommand
        {
            get => _columnRemoveCommand ?? (_columnRemoveCommand = RegisterCommand<ColumnBase>(ColumnRemove, CanColumnRemove));
        }

        private static bool CanColumnRemove(ColumnBase column) => column != null;

        private void ColumnRemove(ColumnBase column) => this.DefaultColumns.Remove(column);

        #endregion

        #region Command: ColumnMoveUpCommand

        private Command<ColumnBase> _columnMoveUpCommand;
        public Command<ColumnBase> ColumnMoveUpCommand
        {
            get => this._columnMoveUpCommand ?? (this._columnMoveUpCommand = this.RegisterCommand<ColumnBase>(this.ColumnMoveUp, this.CanColumnMoveUp));
        }

        private bool CanColumnMoveUp(ColumnBase column)
        {
            return column != null && DefaultColumns.CanItemIndexDecrement(column);
        }

        private void ColumnMoveUp(ColumnBase column)
        {
            this.DefaultColumns.ItemIndexDecrement(column);
            this.ColumnMoveUpCommand.RaiseCanExecute();
            this.ColumnMoveDownCommand.RaiseCanExecute();
        }

        #endregion

        #region Command: ColumnMoveDownCommand

        private Command<ColumnBase> _columnMoveDownCommand;
        public Command<ColumnBase> ColumnMoveDownCommand
        {
            get => this._columnMoveDownCommand ?? (this._columnMoveDownCommand = this.RegisterCommand<ColumnBase>(this.ColumnMoveRight, this.CanColumnMoveRight));
        }

        private bool CanColumnMoveRight(ColumnBase column)
        {
            return column != null && DefaultColumns.CanItemIndexIncrement(column);
        }

        private void ColumnMoveRight(ColumnBase column)
        {
            this.DefaultColumns.ItemIndexIncrement(column);
            this.ColumnMoveDownCommand?.RaiseCanExecute();
            this.ColumnMoveUpCommand.RaiseCanExecute();
        }

        #endregion

        #endregion Commands for columns

        #endregion Accounts

        #region Formats

        public TextBoxController NowPlayingTextBoxController { get; } = new TextBoxController();

        public string NowPlayingFormat
        {
            get => this.Setting.NowPlayingFormat;
            set
            {
                this.Setting.NowPlayingFormat = value;
                this.RaisePropertyChanged(nameof(NowPlayingFormat));
            }
        }

        #region InsertNowPlayingCommand

        public string SelectedNowPlayingParameter
        {
            get => null;
            set
            {
                this.NowPlayingTextBoxController.Insert(value);
                this.NowPlayingTextBoxController.Focus();

                this.RaisePropertyChanged(nameof(this.SelectedNowPlayingParameter));
            }
        }

        private Command<string> _insertNowPlayingCommand;
        public Command<string> InsertNowPlayingParamCommand
        {
            get => _insertNowPlayingCommand ?? (_insertNowPlayingCommand = RegisterCommand<string>(InsertNowPlaying));
        }

        private void InsertNowPlaying(string p)
        {
            NowPlayingTextBoxController.Insert(p);
            NowPlayingTextBoxController.Focus();
        }

        #endregion

        #region ResetNowPlayingCommand

        private Command _resetNowPlayingCommand;
        public Command ResetNowPlayingCommand
        {
            get => _resetNowPlayingCommand ?? (_resetNowPlayingCommand = RegisterCommand(ResetNowPlaying));
        }

        private void ResetNowPlaying() => NowPlayingFormat = Defines.DefaultNowPlayingFormat;

        #endregion

        #endregion

        #region Mute

        public NotifiableCollection<Mute> MuteList { get; } = App.Setting.Mute;

        private MuteType _tempMuteType;
        public MuteType TempMuteType
        {
            get => _tempMuteType;
            set => SetProperty(ref _tempMuteType, value, _muteAddCommand);
        }

        private SearchMode _tempMuteSearch;
        public SearchMode TempMuteSearch
        {
            get => _tempMuteSearch;
            set => SetProperty(ref _tempMuteSearch, value, _muteAddCommand);
        }

        private string _tempMuteText;
        public string TempMuteText
        {
            get => _tempMuteText;
            set => SetProperty(ref _tempMuteText, value, _muteAddCommand);
        }

        private Mute _selectedMute;
        public Mute SelectedMute
        {
            get => _selectedMute;
            set
            {
                if (SetProperty(ref _selectedMute, value))
                {
                    MuteRemoveCommand.RaiseCanExecute();
                }
            }
        }

        #region Command: AddMuteCommand

        private Command _muteAddCommand;
        public Command MuteAddCommand
        {
            get => _muteAddCommand ?? (_muteAddCommand = RegisterCommand(MuteAdd, CanAddMute));
        }

        private void MuteAdd()
        {
            if (Mute.Create(_tempMuteType, _tempMuteSearch, _tempMuteText, out var m))
            {
                MuteList.Add(m);
                SelectedMute = m;
            }
        }

        private bool CanAddMute() => Mute.IsAvailable(_tempMuteType, _tempMuteSearch, _tempMuteText);

        #endregion Command: AddMuteCommand

        #region Command: RemoveMuteCommand

        private Command<Mute> _removeMuteCommand;
        public Command<Mute> RemoveMuteCommand
        {
            get => _removeMuteCommand ?? (_removeMuteCommand = RegisterCommand<Mute>(RemoveMute, IsAvailableMuteItem));
        }

        private void RemoveMute(Mute mute) => MuteList.Remove(mute);

        private static bool IsAvailableMuteItem(Mute mute) => mute != null;

        #endregion Command: RemoveMuteCommand

        #region MuteRemoveCommand

        private Command<Mute> _muteRemoveCommand;
        public Command<Mute> MuteRemoveCommand
        {
            get => _muteRemoveCommand ?? (_muteRemoveCommand = RegisterCommand<Mute>(MuteRemove, CanMuteRemove));
        }

        private void MuteRemove(Mute item) => MuteList.Remove(item);

        private bool CanMuteRemove(Mute item) => MuteList.Contains(item);

        #endregion

        #endregion

        #region Timeline settings

        private const string SampleRelativeTime = "1分前";
        private const string SampleAbsoluteTime = "2018年1月1日 0時0分";

        public string TimelineStatusTimeText
        {
            get
            {
                if (this.Setting.TimelineStatusShowRelativeTime)
                    return SampleRelativeTime;
                else
                    return SampleAbsoluteTime;
            }
        }

        public bool IsShowRelativeTimeTimelineStatus
        {
            get => this.Setting.TimelineStatusShowRelativeTime;
            set
            {
                this.Setting.TimelineStatusShowRelativeTime = value;
                this.RaisePropertiesChanged(
                    nameof(this.IsShowAbsoluteTimeDetailStatus),
                    nameof(this.IsShowRelativeTimeDetailStatus),
                    nameof(this.TimelineStatusTimeText));
            }
        }

        public bool IsShowAbsoluteTimeTimelineStatus
        {
            get => !this.Setting.TimelineStatusShowRelativeTime;
            set
            {
                this.Setting.TimelineStatusShowRelativeTime = !value;
                this.RaisePropertiesChanged(
                    nameof(this.IsShowAbsoluteTimeTimelineStatus),
                    nameof(this.IsShowAbsoluteTimeTimelineStatus),
                    nameof(this.TimelineStatusTimeText));
            }
        }

        public string DetailStatusTimeText
        {
            get
            {
                if (this.Setting.TimelineStatusDetailShowRelativeTime)
                    return SampleRelativeTime;
                else
                    return SampleAbsoluteTime;
            }
        }

        public bool IsShowRelativeTimeDetailStatus
        {
            get => this.Setting.TimelineStatusDetailShowRelativeTime;
            set
            {
                this.Setting.TimelineStatusDetailShowRelativeTime = value;
                this.RaisePropertiesChanged(
                    nameof(this.IsShowAbsoluteTimeDetailStatus),
                    nameof(this.IsShowRelativeTimeDetailStatus),
                    nameof(this.DetailStatusTimeText));
            }
        }

        public bool IsShowAbsoluteTimeDetailStatus
        {
            get => !this.Setting.TimelineStatusDetailShowRelativeTime;
            set
            {
                this.Setting.TimelineStatusDetailShowRelativeTime = !value;
                this.RaisePropertiesChanged(
                    nameof(this.IsShowAbsoluteTimeDetailStatus),
                    nameof(this.IsShowRelativeTimeDetailStatus),
                    nameof(this.DetailStatusTimeText));
            }
        }

        #endregion Timeline settings



        internal override bool CanClose()
        {
            if (AccountManager.Count == 0)
            {
                return DialogService.MessageBox(
                    "アカウントが登録されていません。終了しますか？", "Liberfy",
                    MsgBoxButtons.YesNo, MsgBoxIcon.Question) == MsgBoxResult.Yes;
            }

            if (!EnumerateFontName(this.TimelineFontFamily).Any())
            {
                return DialogService.MessageBox(
                    "フォントが指定されていません。続行しますか？",
                    MsgBoxButtons.YesNo, MsgBoxIcon.Question) == MsgBoxResult.Yes;
            }

            return true;
        }

        internal override void OnClosed()
        {
            this.Setting.DefaultColumns.Clear();
            foreach (var column in this.DefaultColumns)
            {
                this.Setting.DefaultColumns.Add(column.GetOption());
            }

            GlobalSetting.TimelineFont = EnumerateFontName(this.TimelineFontFamily).ToArray();
            GlobalSetting.TimelineFontSize = this.TimelineFontSize.Value;
            GlobalSetting.TimelineFontRendering = this.TimelineFontRenderingMode;

            App.UI.ApplyFromSettings();
        }
    }
}
