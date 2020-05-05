using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace Liberfy.Utils
{
    internal static class DragDropUtil
    {
        public static readonly IReadOnlyList<string> UrlDataFormats = new[]
        {
            "IESiteModeToUrl", "text/x-moz-url", "UniformResourceLocator"
        };

        public static DragDropDataType GetDataType(IDataObject dataObject)
        {
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                return DragDropDataType.FileDrop;
            }
            else if (IsWebBrowserUrl(dataObject))
            {
                return DragDropDataType.Url;
            }
            else if (dataObject.GetDataPresent(DataFormats.Text)
                || dataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                return DragDropDataType.Text;
            }

            return DragDropDataType.Unknown;
        }

        /// <summary>
        /// DataObjectがファイルドロップかどうかを取得する。
        /// </summary>
        /// <param name="dataObject">DataObject</param>
        /// <returns>DataObjectにファイルドロップのデータが含まれるかどうかのフラグ</returns>
        public static bool IsFileDrop(IDataObject dataObject)
            => dataObject.GetDataPresent(DataFormats.FileDrop);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static bool TryGetFileDrop(IDataObject dataObject, out IReadOnlyList<string> paths)
        {
            bool isFileDrop = IsFileDrop(dataObject);

            paths = isFileDrop
                ? (string[])dataObject.GetData(DataFormats.FileDrop)
                : null;

            return isFileDrop;
        }

        public static IReadOnlyList<string> GetFileDrop(IDataObject dataObject)
            => (string[])dataObject.GetData(DataFormats.FileDrop);

        /// <summary>
        /// WebブラウザのURLかどうかを取得する。
        /// </summary>
        /// <param name="dataObject">DataObject</param>
        /// <returns>DataObjectにWebブラウザのURLが含まれているかどうかのフラグ</returns>
        public static bool IsWebBrowserUrl(IDataObject dataObject)
        {
            var formats = dataObject.GetFormats();
            return UrlDataFormats.Any(f => dataObject.GetDataPresent(f))
                && dataObject.GetDataPresent(DataFormats.UnicodeText)
                && dataObject.GetDataPresent(DataFormats.Text);
        }

        public static bool TryGetString(IDataObject dataObject, out string value)
        {
            if (UrlDataFormats.Any(f => dataObject.GetDataPresent(f)))
            {
                value = (string)dataObject.GetData(DataFormats.UnicodeText);
                return true;
            }

            if (dataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                value = (string)dataObject.GetData(DataFormats.UnicodeText);
                return true;
            }

            if (dataObject.GetDataPresent(DataFormats.Text))
            {
                value = (string)dataObject.GetData(DataFormats.Text);
                return true;
            }

            value = null;
            return false;
        }
    }
}
