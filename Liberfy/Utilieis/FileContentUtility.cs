using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace Liberfy.Utilieis
{
    internal static class FileContentUtility
    {
        public static FileStream OpenRead(string path)
        {
            var fileInfo = new FileInfo(path);

            return fileInfo.Exists && fileInfo.Length > 0
                ? fileInfo.OpenRead()
                : default;
        }

        public static FileStream OpenCreate(string path)
        {
            return new FileStream(path, FileMode.Create, FileAccess.Write);
        }
    }
}
