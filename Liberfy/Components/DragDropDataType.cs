using System;
using System.Collections.Generic;
using System.Text;

namespace Liberfy
{
    /// <summary>
    /// ドラッグ＆ドロップ操作時のデータタイプ
    /// </summary>
    public enum DragDropDataType
    {
        /// <summary>
        /// テキスト
        /// </summary>
        Text,

        /// <summary>
        /// WebブラウザのURL
        /// </summary>
        Url,

        /// <summary>
        /// ファイルリスト
        /// </summary>
        FileDrop,

        /// <summary>
        /// 不明／サポート外
        /// </summary>
        Unknown,
    }
}
