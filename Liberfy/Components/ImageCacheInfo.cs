using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Liberfy
{
    internal class ImageCacheInfo : NotificationObject
    {
        private string _fileName;
        public string FileName
        {
            get => this._fileName;
            set => this.SetProperty(ref this._fileName, value);
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get => this._image;
            set => this.SetProperty(ref this._image, value);
        }
    }
}
