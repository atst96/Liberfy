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

namespace Liberfy
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		internal static readonly object CommonLockObject = new object();

		internal static AccountCollection Accounts { get; private set; }
		internal static Setting Setting { get; private set; }

		internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

		public static string ApplicationName { get; } = "Liberfy";

		internal static DictionaryEx<NotifyCode, bool> NotificationEvents { get; } = new DictionaryEx<NotifyCode, bool>();

		protected override void OnStartup(StartupEventArgs e)
		{
			InitializeSettings();

			base.OnStartup(e);
		}

		private static bool InitializeSettings()
		{
			Setting = OpenSettingFile<Setting>(Defines.SettingFile) ?? new Setting();
			Accounts = OpenSettingFile<AccountCollection>(Defines.AccountsFile) ?? new AccountCollection();

			return false;
		}

		private static void SaveSettings()
		{
			SaveJsonFile(Defines.SettingFile, Setting);
			SaveJsonFile(Defines.AccountsFile, Accounts);
		}

		private static bool _appClsoing;

		public static bool Shutdown(bool saveSettings = true)
		{
			if (_appClsoing) return false;
			_appClsoing = true;

			if (saveSettings)
				SaveSettings();

			Current.Shutdown();

			return true;
		}

		private static bool SaveJsonFile(string path, object value)
		{
			byte[] data = Encoding.UTF8.GetBytes(
				JsonConvert.SerializeObject(value, Formatting.Indented));

			FileStream fs = null;

			try
			{
				fs = File.OpenWrite(path);
				fs.SetLength(data.Length);
				fs.Write(data, 0, data.Length);

				return true;
			}
			finally
			{
				data = null;
				fs?.Dispose();
			}
		}

		private static T OpenJsonFile<T>(string path)
		{
			StreamReader sr = null;

			try
			{
				return JsonConvert.DeserializeObject<T>(
					(sr = File.OpenText(path)).ReadToEnd());
			}
			finally
			{
				sr?.Dispose();
			}
		}

		private static T OpenSettingFile<T>(string path) where T : class
		{
			try
			{
				return OpenJsonFile<T>(path);
			}
			catch (FileNotFoundException)
			{
				return null;
			}
			catch (Exception ex)
			{
				MsgBoxResult i;
				switch (i = MessageBox.Show(IntPtr.Zero,
					"設定ファイルの読み込みに失敗しました:\n" + ex.Message,
					"Liberfy", MsgBoxButtons.CancelTryContinue, MsgBoxIcon.Error))
				{
					case MsgBoxResult.TryAgain:
						return OpenSettingFile<T>(path);

					case MsgBoxResult.Continue:
						return Activator.CreateInstance<T>();

					default: ForceExit(); return null;
				}
			}
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
