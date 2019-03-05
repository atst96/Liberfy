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

		public static BitmapImage BitmapSourceFromStream(Stream stream)
		{
			var bitmapSource = new BitmapImage()
			{
				CacheOption = BitmapCacheOption.None,
			};

			bitmapSource.BeginInit();
			bitmapSource.StreamSource = stream;
			bitmapSource.EndInit();

			if (!bitmapSource.CanFreeze)
			{
				bitmapSource.Freeze();
			}

			return bitmapSource;
		}
	}
}
