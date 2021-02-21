using System;
using System.Collections.Generic;
using System.Linq;
using Liberfy.Data.Settings.Columns;

namespace Liberfy.Managers
{
    /// <summary>
    /// カラム管理クラス
    /// </summary>
    internal class ColumnManageer : NotifiableCollection<ColumnBase>, IDisposable
    {
        /// <summary>
        /// アカウント
        /// </summary>
        protected AccountManager Accounts { get; private set; }

        /// <summary>
        /// <see cref="ColumnManageer"/>を生成する。
        /// </summary>
        public ColumnManageer(AccountManager accounts)
        {
            this.Accounts = accounts;
            accounts.Registered += this.OnAccountRegistered;
            accounts.Removed += this.OnAccountRemoved;
        }

        /// <summary>
        /// アカウント追加時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAccountRegistered(object sender, IAccount e)
        {
            var defaultColumns = new IColumnSetting[]
            {
                new HomeColumnSetting(e.ItemId),
                new NotificationColumnSetting(e.ItemId),
            };

            var newColumns = new List<ColumnBase>(defaultColumns.Length);

            foreach (var columnOptions in defaultColumns)
            {
                if (ColumnBase.TryFromSetting(columnOptions, e, out var column))
                {
                    newColumns.Add(column);
                }
            }

            this.AddRange(newColumns);
        }

        /// <summary>
        /// アカウント削除時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAccountRemoved(object sender, IAccount e)
        {
        }

        /// <summary>
        /// カラム設定を復元する。
        /// </summary>
        /// <param name="columns"></param>
        public void Restore(IReadOnlyCollection<IColumnSetting> columns)
        {
            if (columns == null || !columns.Any())
            {
                return;
            }

            var newColumns = new List<ColumnBase>(columns.Count);
            var accountById = this.Accounts.ToDictionary(a => a.ItemId);

            foreach (var columnSetting in columns)
            {
                if (!accountById.TryGetValue(columnSetting.UserId, out var account))
                {
                    continue;
                }

                if (account != null && ColumnBase.TryFromSetting(columnSetting, account, out var column))
                {
                    newColumns.Add(column);
                }
            }

            this.AddRange(newColumns);
        }

        /// <summary>
        /// インスタンスを破棄する。
        /// </summary>
        public void Dispose()
        {
            var accounts = this.Accounts;
            accounts.Registered -= this.OnAccountRegistered;
            accounts.Removed -= this.OnAccountRemoved;
        }
    }
}
