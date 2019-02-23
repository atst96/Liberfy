using Hardcodet.Wpf.TaskbarNotification;
using Liberfy.Settings;
using Microsoft.Win32;
using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    public partial class App : Application
    {
        internal const bool __DEBUG_LoadTimeline = true;

        internal static Setting Setting { get; private set; }

        public static ApplicationStatus Status { get; } = new ApplicationStatus();

        internal static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        public const string AppName = "Liberfy";
        public const string AppCodeName = "Francium";
        public const string AppVersion = "0.1.2.0";

        internal static DictionaryEx<NotifyCode, bool> NotificationEvents { get; } = new DictionaryEx<NotifyCode, bool>();

        internal static TaskbarIcon TaskbarIcon { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 作業ディレクトリの再指定（自動起動時に作業ディレクトリが変わってしまう対策）
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.Location));

            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(IntPtr.Zero, ex.GetMessage(), "Liberfy", icon: MsgBoxIcon.Error);
                Environment.Exit(1);
                return;
            }

            InitializeProgram();
            StartTimeline();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SystemEvents.SessionEnding -= OnSystemSessionEnding;

            App.SaveSettings();

            base.OnExit(e);
        }

        private static void LoadSettings()
        {
            var loadAccountsTask = ParseSettingAsync<AccountSetting>(Defines.AccountsFile);
            var loadSettingTask = ParseSettingAsync<Setting>(Defines.SettingFile);

            Task.WaitAll(loadAccountsTask, loadSettingTask);

            // 登録アカウントの読み込み
            var accountsSetting = loadAccountsTask.Result;

            if (accountsSetting != null)
            {
                var (accounts, columns) = (accountsSetting.Accounts, accountsSetting.Columns);

                if (accounts?.Any() ?? false)
                {
                    LoadAccounts(accounts);
                }

                if (columns?.Any() ?? false)
                {
                    LoadColumns(columns);
                }
            }

            // 設定の読み込み
            Setting = loadSettingTask.Result ?? new Setting();
        }

        private static void InitializeProgram()
        {
            UI.ApplyFromSettings();

            foreach (var muteItem in Setting.Mute.AsParallel())
            {
                muteItem.Apply();
            }

            TaskbarIcon = GetResource<TaskbarIcon>("taskbarIcon");

            if (AccountManager.Count == 0 && !RequestInitialUserSettings())
            {
                App.Shutdown(false);
                return;
            }
        }

        private static void LoadAccounts(IEnumerable<AccountItem> accounts)
        {
            foreach (var accountSetting in accounts.Distinct())
            {
                AccountManager.Add(AccountBase.FromSetting(accountSetting));
            }
        }

        private static void LoadColumns(IEnumerable<ColumnSetting> columns)
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

        private static bool _isAccountLoaded;

        private static void StartTimeline()
        {
            if (_isAccountLoaded)
            {
                return;
            }

            _isAccountLoaded = true;

            var tasks = AccountManager.Accounts.AsParallel().Select(a => a.Load());

            Task.WhenAll(tasks).ContinueWith(_ => Status.IsAccountLoaded = true, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private static bool RequestInitialUserSettings()
        {
            var sw = new SettingWindow();

            sw.MoveTabPage(1);

            sw.ShowDialog();

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
                return await FileContentUtility.DeserializeJsonFileAsync<T>(filename).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定ファイル \"{filename}\" の読み込みに失敗しました。\n{ex.GetMessage()}", ex);
            }
        }

        public static void DisplayException(Exception ex, string instruction = null)
        {
            const string NewLine = "\n";

            var message = string.IsNullOrEmpty(instruction)
                ? string.Empty
                : instruction + NewLine;

            message += string.Join(NewLine, ex.Message, ex.StackTrace);

            MessageBox.Show(IntPtr.Zero, message, "エラー", icon: MsgBoxIcon.Error);
        }

        private static void SaveSettings()
        {
            var accountsSetting = new AccountSetting
            {
                Accounts = AccountManager.Accounts.Select(a => a.ToSetting()),
                Columns = TimelineBase.Columns.Select(c => c.GetOption()),
            };

            var saveSettingTask = Task.Run(() => SaveSetting(Defines.SettingFile, App.Setting));
            var saveAccountsTask = Task.Run(() => SaveSetting(Defines.AccountsFile, accountsSetting));

            try
            {
                Task.WaitAll(saveSettingTask, saveAccountsTask);
            }
            catch (Exception ex)
            {
                MessageBox.Show(IntPtr.Zero, ex.GetMessage(), App.AppName, icon: MsgBoxIcon.Error);
            }
        }

        private static void SaveSetting<T>(string filename, T setting) where T : class
        {
            try
            {
                FileContentUtility.SerializeJsonToFile(setting, filename);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定 \"{filename}\" の保存に失敗しました。\n{ex.GetMessage()}", ex);
            }
        }

        private static bool _appClsoing;
        private static bool _saveSetting = true;

        public static void Shutdown(bool saveSettings)
        {
            if (_appClsoing) return;
            _appClsoing = true;

            _saveSetting = saveSettings;

            Current.Shutdown();
        }

        public static Visibility BoolToVisibility(bool isVisible)
        {
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public static T GetResource<T>(object resourceKey)
        {
            return Current.TryFindResource(resourceKey) is T value ? value : default;
        }

        public static bool SetResource(object key, object value)
        {
            if (Current.TryFindResource(key) != value)
            {
                Current.Resources[key] = value;

                return true;
            }

            return false;
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
    }
}
