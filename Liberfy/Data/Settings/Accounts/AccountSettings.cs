using Liberfy.Data.Settings.Columns;
using MessagePack;
using System.Collections.Generic;

namespace Liberfy.Settings
{
    [MessagePackObject]
    internal class AccountSettings
    {
        [Key(0)]
        public IReadOnlyList<IAccountSetting> Accounts { get; init; }

        [Key(1)]
        public IReadOnlyList<IColumnSetting> Columns { get; init; }
    }
}
