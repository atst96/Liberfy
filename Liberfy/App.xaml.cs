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
			if (LoadSettingWithErrorDialog(Defines.AccountsFile, ref _accounts))
			{
				if(_accounts.Columns != null)
				{
					foreach(var s in _accounts.Columns)
					{
						try
						{
							Columns.Add(s.ToColumn());
						}
						catch (Exception ex)
						{
						}
					}
				}
			}
			else
			{
				App.Shutdown(false);
				return;
			}

			if (!LoadSettingWithErrorDialog(Defines.SettingFile, ref _setting))
			{
				App.Shutdown(false);
				return;
			}

			TaskbarIcon = (TaskbarIcon)FindResource("taskbarIcon");

			base.OnStartup(e);
		}

		internal static FluidCollection<ColumnBase> Columns { get; } = new FluidCollection<ColumnBase>();

		private static bool LoadSettingWithErrorDialog<T>(string filename, ref T setting) where T : new()
		{
			try
			{
				setting = File.Exists(filename)
					? SettingFromFile<T>(filename)
					: new T();

				return true;
			}
			catch (Exception e)
			{
				MessageBox.Show(IntPtr.Zero,
					$"設定ファイルの{(e is JsonException ? "保存形式が誤っています" : "読み込みに失敗しました")}：\n"
					+ $"{e.Message}\n\nアプリケーションを終了します。",
					caption: "エラー", icon: MsgBoxIcon.Error);
				return true;
			}
		}

		private static T SettingFromFile<T>(string filename)
		{
			return JsonConvert.DeserializeObject<T>(
				File.ReadAllText(filename));
		}

		private static void SaveSettingFile<T>(string filename, T TObj)
		{
			File.WriteAllText(filename,
				JsonConvert.SerializeObject(TObj, Formatting.Indented));
		}

		private static void SaveSettings()
		{
			// 各設定の保存
			AccountSetting.Columns
				= Columns.Select(c => c.ToSetting()).ToArray();

			// 設定をファイルに保存
			SaveSettingWithErrorDialog(Defines.SettingFile, Setting);
			SaveSettingWithErrorDialog(Defines.AccountsFile, AccountSetting);
		}

		private static void SaveSettingWithErrorDialog<T>(string filename, T setting) where T : class
		{
			try
			{
				SaveSettingFile(filename, setting);
			}
			catch (Exception e)
			{
				MessageBox.Show(IntPtr.Zero,
					$"設定ファイルの保存に失敗しました。：\n{e.Message}",
					caption: "エラー", icon: MsgBoxIcon.Error);
			}
		}

		private static bool _appClsoing;

		public static void Shutdown(bool saveSettings = true)
		{
			if (_appClsoing) return;
			_appClsoing = true;

			if (saveSettings)
			{
				App.SaveSettings();
			}

			Current.Shutdown();
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
