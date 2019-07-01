using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Liberfy.Utilities
{
    internal static class ImageUtility
    {
        public static bool IsAnimatedGif(Uri path)
        {
            var decoder = BitmapDecoder.Create(path, BitmapCreateOptions.None, BitmapCacheOption.None);

            return decoder is GifBitmapDecoder && decoder.Frames.Count > 1;
        }

        public static BitmapImage CreateImage(Stream stream)
        {
            var bitmapSource = new BitmapImage();

            bitmapSource.BeginInit();
            bitmapSource.CreateOptions = BitmapCreateOptions.None;
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.StreamSource = stream;
            bitmapSource.EndInit();

            if (bitmapSource.CanFreeze)
            {
                bitmapSource.Freeze();
            }

            return bitmapSource;
        }

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
