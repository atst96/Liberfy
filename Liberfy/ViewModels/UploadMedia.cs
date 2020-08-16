using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static Liberfy.Defaults;
using System.Drawing;
using SocialApis.Twitter;
using SocialApis;
using Liberfy.Utils;
using SocialApis.Core;

namespace Liberfy.ViewModels
{
    internal sealed class UploadMedia : NotificationObject, IProgress<UploadReport>, IDisposable
    {
        public string Path { get; private set; }
        public MediaType MediaType { get; }
        public string DisplayExtension { get; }
        public byte[] RawData { get; private set; }
        public BitmapImage PreviewImage { get; private set; }

        private UploadMedia(BitmapSource bitmapSource, MediaType mediaType, string ext)
        {
            this.RawData = bitmapSource.ToByteArray<PngBitmapEncoder>();
            this.PreviewImage = ImageUtil.CreateBitmapImage(this.GetDataStream());
            this.MediaType = mediaType;
            this.DisplayExtension = ext;
        }

        private UploadMedia(string path)
        {
            var ext = System.IO.Path.GetExtension(path);

            if (ImageExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                var uri = new Uri(path, UriKind.Absolute);

                if (ext.Equals(".gif", StringComparison.OrdinalIgnoreCase) && ImageUtil.IsAnimatedGif(uri))
                {
                    this.MediaType = MediaType.AnimatedGifFile;
                    this.PreviewImage = ImageUtil.FromFile(uri);
                }
                else
                {
                    this.MediaType = MediaType.ImageFile;

                    if (!ext.Equals(".webp", StringComparison.OrdinalIgnoreCase))
                    {
                        this.PreviewImage = ImageUtil.FromFile(uri);
                    }
                }
            }
            else if (VideoExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                this.MediaType = MediaType.VideoFile;
            }
            else
            {
                throw new NotSupportedException();
            }

            this.Path = path;
            this.DisplayExtension = ext.Substring(1).ToUpper();

            if (this.PreviewImage?.CanFreeze ?? false)
            {
                this.PreviewImage.Freeze();
            }
        }

        public static UploadMedia FromBitmapSource(BitmapSource image, string displayExtension)
        {
            return new UploadMedia(image, MediaType.Image, displayExtension);
        }

        public static UploadMedia FromFile(string filepath)
        {
            return new UploadMedia(filepath);
        }

        private string _description;
        public string Description
        {
            get => this._description;
            private set => this.SetProperty(ref this._description, value);
        }

        private double _uploadProgress;
        public double UploadProgress
        {
            get => this._uploadProgress;
            private set => this.SetProperty(ref this._uploadProgress, value);
        }

        private bool _isUploading;
        public bool IsUploading
        {
            get => this._isUploading;
            private set => this.SetProperty(ref this._isUploading, value);
        }

        public Stream GetDataStream()
        {
            if (this.RawData == null)
            {
                return File.OpenRead(this.Path);
            }
            else
            {
                return new MemoryStream(this.RawData, false);
            }
        }

        public void BeginUpload()
        {
            this.UploadProgress = 0.0d;
            this.IsUploading = true;
        }

        public void EndUpload()
        {
            this.IsUploading = false;
            this.UploadProgress = 0.0d;
        }

        public void Report(UploadReport value)
        {
            this.UploadProgress = value.UploadPercentage;
        }

        public void Dispose()
        {
            this.RawData = null;
            this.PreviewImage = null;
            this.Path = null;
        }

        public static class DisplayExtensions
        {
            public const string Clipboard = "CLPB";
            public const string Artwork = "ARTW";
        }
    }

    [Flags]
    internal enum MediaType : uint
    {
        None = 0,
        File = 0x01,
        Image = 0x02,
        Video = 0x04,
        AnimatedGif = 0x08,
        ImageFile = Image | File,
        VideoFile = Video | File,
        AnimatedGifFile = AnimatedGif | File,
    }
}
