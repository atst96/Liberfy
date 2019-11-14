using Hardcodet.Wpf.TaskbarNotification;
using Liberfy.Components;
using Liberfy.Settings;
using Liberfy.Utilieis;
using Liberfy.ViewModels;
using Liberfy.Views;
using Microsoft.Win32;
using Microsoft.Windows.Themes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Utf8Json;

namespace Liberfy
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    internal partial class App : Application
    {
        internal const bool __DEBUG_LoadTimeline = true;

        internal static App Instance { get; private set; }

        internal static Setting Setting { get; private set; }

        public static ApplicationStatus Status { get; } = new ApplicationStatus();

        internal static readonly Assembly AssemblyInfo = Assembly.GetExecutingAssembly();

        private static Database _cacheDatabaseConnection;
        public static ProfileImageCache ProfileImageCache { get; private set; }

        public const string Name = "Liberfy";
        public const string CodeName = "Francium";
        public const string Version = "0.2.3.1";

        public bool IsRequireSaveSetting { get; private set; } = true;

        internal static DictionaryEx<NotifyCode, bool> NotificationEvents { get; } = new DictionaryEx<NotifyCode, bool>();

        internal static TaskbarIcon TaskbarIcon { get; private set; }

        public UISettingManager UIManager { get; private set; }

        public App() : base()
        {
            App.Instance = (App)Application.Current;
        }

        public static string GetLocalDirectory()
        {
            return Path.GetDirectoryName(AssemblyInfo.Location);
        }

        public static string GetLocalFilePath(string filename)
        {
            return Path.Combine(GetLocalDirectory(), filename);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 作業ディレクトリの再指定（自動起動時に作業ディレクトリが変わってしまう対策）
            Directory.SetCurrentDirectory(GetLocalDirectory());

            try
            {
                this.LoadSettings();
            }
            catch (Exception ex)
            {
                DialogService.ShowTaskDialog(IntPtr.Zero, ex.GetMessage(), null, null, TaskDialogStandardButtons.Close, TaskDialogStandardIcon.Error); ;
                Debug.WriteLine(ex.StackTrace);
                this.ForceShutdown();
                return;
            }

            this.UIManager = new UISettingManager(this, App.Setting);
            this.UIManager.Apply();

            foreach (var muteItem in Setting.Mute.AsParallel())
            {
                muteItem.Apply();
            }

            TaskbarIcon = this.TryFindResource("taskbarIcon") as TaskbarIcon;

            if (AccountManager.Count == 0 && !this.RequestInitialUserSettings())
            {
                this.ForceShutdown();
                return;
            }

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol =
                 SecurityProtocolType.Tls
                 | SecurityProtocolType.Tls11
                 | SecurityProtocolType.Tls12
                 | SecurityProtocolType.Tls13;

            this.StartTimeline();
        }

        private void LoadSettings()
        {
            var loadAccountsTask = ParseSettingAsync<AccountSettings>(GetLocalFilePath(Defaults.AccountsFile));
            var loadSettingTask = ParseSettingAsync<Setting>(GetLocalFilePath(Defaults.SettingFile));

            Task.WaitAll(loadAccountsTask, loadSettingTask);

            // 登録アカウントの読み込み
            var accountsSetting = loadAccountsTask.Result;

            if (accountsSetting != null)
            {
                var (accounts, columns) = (accountsSetting.Accounts, accountsSetting.Columns);

                if (accounts?.Any() ?? false)
                {
                    this.InitializeSavedAccounts(accounts);
                }

                if (columns?.Any() ?? false)
                {
                    this.InitializeSavedColumns(columns);
                }
            }

            // 設定の読み込み
            Setting = loadSettingTask.Result ?? new Setting();
        }

        private void InitializeSavedAccounts(IEnumerable<AccountSettingBase> accounts)
        {
            foreach (var accountSetting in accounts.Distinct())
            {
                AccountManager.Add(AccountBase.FromSetting(accountSetting));
            }
        }

        private void InitializeSavedColumns(IEnumerable<ColumnSetting> columns)
        {
            foreach (var columnSetting in columns)
            {
                var account = AccountManager.Get(columnSetting.Service, columnSetting.UserId);

                if (account != null && ColumnBase.FromSetting(columnSetting, account, out var column))
                {
                    TimelineBase.Columns.Add(column);
                }
            }
        }

        private void StartTimeline()
        {
            _cacheDatabaseConnection = new Database(Defaults.ImageCacheFile);

            ProfileImageCache = new ProfileImageCache(_cacheDatabaseConnection);

            ProfileImageCache.BeginLoadTimelineMode();

            var tasks = AccountManager.Accounts.AsParallel().Select(a => a.Load());

            Task.WhenAll(tasks).ContinueWith(_ =>
            {
                Status.IsAccountLoaded = true;
                ProfileImageCache.EndLoadTimelineMode();
            },
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        private bool RequestInitialUserSettings()
        {
            var tempShutdownMode = this.ShutdownMode;
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var settingWindow = new SettingWindow();
            settingWindow.MoveTabPage(1);
            settingWindow.ShowDialog();

            this.ShutdownMode = tempShutdownMode;

            return AccountManager.Count > 0;
        }

        private void OnSystemSessionEnding(object sender, SessionEndingEventArgs e)
        {
            if (e.Reason == SessionEndReasons.Logoff)
            {
                e.Cancel = Setting.SystemCancelSignout;
            }
            else if (e.Reason == SessionEndReasons.SystemShutdown)
            {
                e.Cancel = Setting.SystemCancelShutdown;
            }
        }

        private static async Task<T> ParseSettingAsync<T>(string filename) where T : class
        {
            try
            {
                return await JsonUtility.DeserializeFileAsync<T>(filename).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定ファイル \"{filename}\" の読み込みに失敗しました。\n{ex.GetMessage()}", ex);
            }
        }

        private void SaveSettings()
        {
            var accountsSetting = new AccountSettings
            {
                Accounts = AccountManager.Accounts.Select(a => a.ToSetting()),
                Columns = TimelineBase.Columns.Select(c => c.GetOption()),
            };

            var saveSettingTask = SaveSetting(GetLocalFilePath(Defaults.SettingFile), App.Setting);
            var saveAccountsTask = SaveSetting(GetLocalFilePath(Defaults.AccountsFile), accountsSetting);

            try
            {
                Task.WaitAll(saveSettingTask, saveAccountsTask);
            }
            catch (Exception ex)
            {
                DialogService.ShowTaskDialog(IntPtr.Zero, ex.GetMessage(), null, null, TaskDialogStandardButtons.Close, TaskDialogStandardIcon.Error);
            }
        }

        private static async Task SaveSetting<T>(string filename, T setting) where T : class
        {
            try
            {
                await JsonUtility.SerializeFileAsync(setting, filename).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定 \"{filename}\" の保存に失敗しました。\n{ex.GetMessage()}", ex);
            }
        }

        public void ForceShutdown()
        {
            this.IsRequireSaveSetting = true;

            this.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SystemEvents.SessionEnding -= this.OnSystemSessionEnding;

            App._cacheDatabaseConnection?.Dispose();

            if (this.IsRequireSaveSetting)
            {
                this.SaveSettings();
            }

            base.OnExit(e);
        }

        internal static bool Open(string path)
        {
            try
            {
                Process.Start(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static bool Open(Uri uri)
        {
            return Open(uri.AbsoluteUri);
        }

        public T FindResource<T>(object resourceKey)
        {
            return this.FindResource(resourceKey).CastOrDefault<T>();
        }

        public T TryFindResource<T>(object resourceKey)
        {
            return this.TryFindResource(resourceKey).CastOrDefault<T>();
        }

        public IEnumerable<Window> EnumerateWindows()
        {
            return this.Windows.Cast<Window>();
        }

        public IEnumerable<T> FindViewModel<T>() where T : ViewModelBase
        {
            return this.EnumerateWindows()
                .Select(w => w.DataContext is T viewModel ? viewModel : default)
                .Where(vm => vm != null);
        }

        public IEnumerable<Window> FindViewModelWindow<T>() where T : ViewModelBase
        {
            return this.EnumerateWindows()
                .Where(w => w.DataContext is T);
        }

        public IEnumerable<(Window view, T viewModel)> FindViewModelWithWindow<T>() where T : ViewModelBase
        {
            foreach (var window in this.EnumerateWindows())
            {
                if (window.DataContext is T viewModel)
                {
                    yield return (window, viewModel);
                }
            }
        }
    }
}
