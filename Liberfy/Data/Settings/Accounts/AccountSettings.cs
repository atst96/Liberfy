using Liberfy.Data.Settings.Columns;
using MessagePack;
using System.Collections.Generic;

namespace Liberfy.Settings
{
    /// <summary>
    /// アカウント設定情報
    /// </summary>
    [MessagePackObject]
    internal class AccountSettings
    {
        [Key("accounts")]
        public IReadOnlyList<IAccountSetting> Accounts { get; init; }

        [Key("columns")]
        public IReadOnlyList<IColumnSetting> Columns { get; init; }
    }
}
