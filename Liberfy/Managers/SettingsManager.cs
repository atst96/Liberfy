#nullable enable

using System;
using System.Threading.Tasks;
using Liberfy.Data.Settings;
using Liberfy.Settings;
using Liberfy.Utilieis;
using Liberfy.Utils;

namespace Liberfy.Managers
{
    internal static class SettingsManager
    {
        /// <summary>
        /// すべての設定ファイルを読み込む。
        /// </summary>
        /// <returns></returns>
        public static async Task<SettingsGroup> Load()
        {
            var applicationTask = LoadAppSettings();
            var accountsTask = LoadAccounts();

            Task[] tasks =
            {
                applicationTask,
                accountsTask,
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return new()
            {
                Application = applicationTask.Result ?? new(),
                Accounts = accountsTask.Result ?? new(),
            };
        }

        /// <summary>
        /// アカウント設定ファイルを読み込む
        /// </summary>
        /// <returns></returns>
        public static Task<Setting> LoadAppSettings()
        {
            var filePath = App.GetLocalFilePath(Defaults.SettingFile);

            return ParseJsonSettingAsync<Setting>(filePath);
        }

        /// <summary>
        /// 一般設定ファイルを読み込む
        /// </summary>
        /// <returns></returns>
        public static Task<AccountSettings> LoadAccounts()
        {
            var filePath = App.GetLocalFilePath(Defaults.AccountsFile);

            return ParseMessagePackSettingAsync<AccountSettings>(filePath);
        }

        /// <summary>
        /// 設定ファイルを読み込む。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static async Task<T> ParseMessagePackSettingAsync<T>(string filename) where T : class
        {
            try
            {
                return await MessagePackUtil.DeserializeFileAsync<T>(filename).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定ファイル \"{filename}\" の読み込みに失敗しました。\n{ex.GetMessage()}", ex);
            }
        }

        /// <summary>
        /// 設定ファイルを読み込む。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static async Task<T> ParseJsonSettingAsync<T>(string filename) where T : class
        {
            try
            {
                return await JsonUtil.DeserializeFileAsync<T>(filename).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定ファイル \"{filename}\" の読み込みに失敗しました。\n{ex.GetMessage()}", ex);
            }
        }

        /// <summary>
        /// すべての設定ファイルを保存する。
        /// </summary>
        /// <returns>Task</returns>
        public static Task Save(SettingsGroup settings)
        {
            return Task.WhenAll(
                SaveAppSettings(settings.Application),
                SaveAccounts(settings.Accounts));
        }

        /// <summary>
        /// 設定データを保存する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        private static async Task SaveJsonSetting<T>(string filename, T setting) where T : class
        {
            try
            {
                await JsonUtil.SerializeFileAsync(setting, filename).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定 \"{filename}\" の保存に失敗しました。\n{ex.GetMessage()}", ex);
            }
        }

        /// <summary>
        /// 設定データを保存する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        private static async Task SaveMessagePackSetting<T>(string filename, T setting) where T : class
        {
            try
            {
                await MessagePackUtil.SerializeFileAsync(setting, filename).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定 \"{filename}\" の保存に失敗しました。\n{ex.GetMessage()}", ex);
            }
        }

        /// <summary>
        /// アカウント設定をファイルに保存する。
        /// </summary>
        /// <param name="accountSettings">アカウント設定</param>
        /// <returns></returns>
        public static Task SaveAccounts(AccountSettings accountSettings)
        {
            var filePath = App.GetLocalFilePath(Defaults.AccountsFile);

            return SaveMessagePackSetting(filePath, accountSettings);
        }

        /// <summary>
        /// 一般設定をファイルに保存する。
        /// </summary>
        /// <param name="setting">設定</param>
        /// <returns></returns>
        public static Task SaveAppSettings(Setting setting)
        {
            var filePath = App.GetLocalFilePath(Defaults.SettingFile);

            return SaveJsonSetting(filePath, setting);
        }
    }
}
