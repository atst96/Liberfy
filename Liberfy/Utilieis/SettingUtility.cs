using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liberfy.Settings;

namespace Liberfy.Utilieis
{
    internal static class SettingUtility
    {
        /// <summary>
        /// すべての設定ファイルを読み込む。
        /// </summary>
        /// <returns></returns>
        public static async Task<(Setting, AccountSettings)> LoadSettings()
        {
            var generalSettingTask = SettingUtility.LoadGeneralSettings();
            var accountSettingTask = SettingUtility.LoadAccountsSetting();

            await Task.WhenAll(generalSettingTask, accountSettingTask).ConfigureAwait(false);

            return (
                generalSettingTask.Result ?? new Setting(),
                accountSettingTask.Result ?? new AccountSettings());
        }

        /// <summary>
        /// アカウント設定ファイルを読み込む
        /// </summary>
        /// <returns></returns>
        private static Task<Setting> LoadGeneralSettings()
        {
            var filePath = App.GetLocalFilePath(Defaults.SettingFile);

            return ParseSettingAsync<Setting>(filePath);
        }

        /// <summary>
        /// 一般設定ファイルを読み込む
        /// </summary>
        /// <returns></returns>
        private static Task<AccountSettings> LoadAccountsSetting()
        {
            var filePath = App.GetLocalFilePath(Defaults.AccountsFile);

            return ParseSettingAsync<AccountSettings>(filePath);
        }

        /// <summary>
        /// 設定ファイルを読み込む。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
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

        /// <summary>
        /// すべての設定ファイルを保存する。
        /// </summary>
        /// <returns>Task</returns>
        public static Task SaveSettings(Setting generalSetting, AccountSettings accountSetting)
        {
            return Task.WhenAll(
                SettingUtility.SaveGeneralSettings(generalSetting),
                SettingUtility.SaveAccountsSettings(accountSetting));
        }

        /// <summary>
        /// 設定データを保存する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
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

        /// <summary>
        /// アカウント設定をファイルに保存する。
        /// </summary>
        /// <param name="accountSettings">アカウント設定</param>
        /// <returns></returns>
        private static Task SaveAccountsSettings(AccountSettings accountSettings)
        {
            var filePath = App.GetLocalFilePath(Defaults.AccountsFile);

            return SettingUtility.SaveSetting(filePath, accountSettings);
        }

        /// <summary>
        /// 一般設定をファイルに保存する。
        /// </summary>
        /// <param name="setting">設定</param>
        /// <returns></returns>
        private static Task SaveGeneralSettings(Setting setting)
        {
            var filePath = App.GetLocalFilePath(Defaults.SettingFile);

            return SettingUtility.SaveSetting(filePath, setting);
        }
    }
}
