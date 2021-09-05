using System;
using System.Diagnostics;

namespace Liberfy
{
    /// <summary>
    /// 例外に関する拡張メソッド群
    /// </summary>
    internal static class ExceptionExtensions
    {
        /// <summary>
        /// 例外の内容をデバッグコンソールに出力する
        /// </summary>
        /// <param name="e"></param>
        public static void DumpDebug(this Exception e)
        {
            Debug.WriteLine("== 例外スタックトレース開始 ==");
            Debug.WriteLine(e.Message);
            Debug.WriteLine(e.StackTrace);
            Debug.WriteLine("== 例外スタックトレース終了 ==");
        }
    }
}
