#nullable enable

using MessagePack;

namespace Liberfy.Data.Settings.Columns
{
    [MessagePackObject]
    internal class HomeColumnSetting : IColumnSetting
    {
        /// <summary>
        /// 内部ユーザID
        /// </summary>
        [Key(0)]
        public string UserId { get; init; }

        /// <summary>
        /// カラム種別
        /// </summary>
        [Key(1)]
        public ColumnType Type { get; init; }

        [SerializationConstructor]
        private HomeColumnSetting()
        {
        }

        public HomeColumnSetting(string userId)
        {
            this.UserId = userId;
        }
    }
}
