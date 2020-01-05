using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace Liberfy.Utilieis
{
    /// <summary>
    /// ファイル操作に関するクラス
    /// </summary>
    internal static class FileContentUtility
    {
        /// <summary>
        /// ファイルを読み取りモードで開く
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="isAsync">非同期モードで開くかどうかのフラグ</param>
        /// <returns>ファイルストリーム</returns>
        public static FileStream OpenRead(string path, bool isAsync = false)
        {
            var fileInfo = new FileInfo(path);

            return fileInfo.Exists && fileInfo.Length > 0
                ? new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, (int)fileInfo.Length, isAsync)
                : default;
        }

        /// <summary>
        /// ファイルを書き込みモードで開く
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="isAsync">非同期モードで開くかどうかのフラグ</param>
        /// <param name="bufferSize">バッファーサイズ</param>
        /// <returns>ファイルストリーム</returns>
        public static FileStream OpenCreate(string path, bool isAsync = false, int? bufferSize = null)
        {
            const int DefaultBufferSize = 4096;

            return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, bufferSize ?? DefaultBufferSize, isAsync);
        }
    }
}
