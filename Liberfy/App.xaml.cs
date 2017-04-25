using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Windows.Themes;
using Newtonsoft.Json;
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
using System.Windows.Documents;

namespace Liberfy
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		internal static readonly object CommonLockObject = new object();

		private static AccountSetting _accounts;
		private static Setting _setting;

		internal static AccountSetting AccountSetting => _accounts;
		internal static Setting Setting => _setting;

		private static Assembly _assembly = Assembly.GetExecutingAssembly();
		internal static Assembly Assembly => _assembly;

		public static string ApplicationName { get; } = "Liberfy";

		internal static DictionaryEx<NotifyCode, bool> NotificationEvents { get; } = new DictionaryEx<NotifyCode, bool>();

		internal static TaskbarIcon TaskbarIcon { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			// 設定を読み込む
			// InitializeSettings();
			if (!loadSettingWithErrorDialog(Defines.SettingFile, ref _setting))
			{
				App.Shutdown(false);
				return;
			}

			if (!loadSettingWithErrorDialog(Defines.AccountsFile, ref _accounts))
			{
				App.Shutdown(false);
				return;
			}

			TaskbarIcon = (TaskbarIcon)FindResource("taskbarIcon");

			base.OnStartup(e);
		}

		private static bool loadSettingWithErrorDialog<T>(string fileName, ref T setting) where T : SettingBase
		{
			var response = SettingBase.FromFile<T>(fileName);

			switch (response.Status)
			{
				case FileProcessStatus.Success:
					setting = response.Result;
					return true;

				case FileProcessStatus.FileNotFound:
					setting = Activator.CreateInstance<T>();
					return true;

				default:
					string instruction = response.Status == FileProcessStatus.ParseError ? "保存形式が誤っています" : "読み込みに失敗しました";
					MessageBox.Show(IntPtr.Zero, $"設定ファイルの{instruction}：\n{response.ErrorMessage}\n\nアプリケーションを終了します。", caption: "エラー", icon: MsgBoxIcon.Error);
					return false;
			}
		}

		private static void saveSettings()
		{
			saveSettingWithErrorDialog(Defines.SettingFile, Setting);
			saveSettingWithErrorDialog(Defines.AccountsFile, AccountSetting);
		}

		private static void saveSettingWithErrorDialog<T>(string fileName, T setting) where T : SettingBase
		{
			var response = SettingBase.SaveFile(fileName, setting);

			if(response.Status != FileProcessStatus.Success)
			{
				MessageBox.Show(IntPtr.Zero,
					$"設定ファイルの保存に失敗しました。：\n{response.ErrorMessage}",
					caption: "エラー", icon: MsgBoxIcon.Error);
			}
		}

		private static bool _appClsoing;

		public static bool Shutdown(bool saveSettings = true)
		{
			if (_appClsoing) return false;
			_appClsoing = true;

			if (saveSettings)
				App.saveSettings();

			Current.Shutdown();

			return true;
		}

		public static void LoadUISettingsFromSetting()
		{

		}

		internal static void ForceExit()
		{
			// TODO: 仮処理
			Environment.Exit(0);
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
