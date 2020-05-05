using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Liberfy.Utils
{
    /// <summary>
    /// 画像処理に関するクラス
    /// </summary>
    internal static class ImageUtil
    {
        /// <summary>
        /// アニメーションGIFかどうかを判定する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns>アニメーションGIFかどうか</returns>
        public static bool IsAnimatedGif(Uri path)
        {
            var decoder = BitmapDecoder.Create(path, BitmapCreateOptions.None, BitmapCacheOption.None);

            return decoder is GifBitmapDecoder && decoder.Frames.Count > 1;
        }

        /// <summary>
        /// StreamからBitmapImageを生成する。
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage CreateBitmapImage(Stream stream)
        {
            var image = new BitmapImage();

            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.None;
            image.CacheOption = BitmapCacheOption.None;
            image.StreamSource = stream;
            image.EndInit();

            if (image.CanFreeze)
            {
                image.Freeze();
            }

            return image;
        }

        /// <summary>
        /// UriからBitmapImageを読み込む。
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage FromFile(Uri uri)
        {
            var bitmapImage = new BitmapImage(uri);

            if (bitmapImage.CanFreeze)
            {
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }
    }
}
