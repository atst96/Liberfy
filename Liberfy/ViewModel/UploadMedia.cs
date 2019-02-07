using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static Liberfy.Defines;
using System.Drawing;
using SocialApis.Twitter;
using SocialApis;

namespace Liberfy.ViewModel
{
	internal class UploadMedia : NotificationObject, IProgress<UploadProgress>, IDisposable
	{
		private UploadMedia(BitmapSource bmpSource, MediaType mediaType, string ext)
		{
			var pngEnc = new PngBitmapEncoder();
			var memStr = new MemoryStream();

			// 画像データをストリームに保存
			PreviewImage = new BitmapImage();
			pngEnc.Frames.Add(BitmapFrame.Create(bmpSource));
			pngEnc.Save(memStr);
			pngEnc = null;

			// ストリームからプレビュー画像の生成
			memStr.Position = 0;
			SourceStream = memStr;
			PreviewImage.BeginInit();
			PreviewImage.StreamSource = memStr;
			PreviewImage.EndInit();

			MediaType = mediaType;
			ViewExtension = ext;
		}

		private UploadMedia(string filePath)
		{
			var ext = Path.GetExtension(filePath).ToLower();

			if (ext.Equals(".gif") && IsAnimatedGif(ext))
			{
				this.MediaType = MediaType.AnimatedGifFile;
				this.PreviewImage = new BitmapImage(new Uri(filePath));
			}
			else if (VideoExtensions.Contains(ext))
			{
				this.MediaType = MediaType.VideoFile;
				this.UseChunkedUpload = true;
			}
			else if (ImageExtensions.Contains(ext))
			{
				this.MediaType = MediaType.ImageFile;

				if (ext != ".webp")
				{
					this.PreviewImage = new BitmapImage(new Uri(filePath));
				}
			}
			else
			{
				throw new NotSupportedException();
			}

			this.FilePath = filePath;
			filePath = Path.GetFileName(filePath);
			this.ViewExtension = ext.Substring(1).ToUpper();
			this.SourceStream = File.OpenRead(this.FilePath);

			if (this.PreviewImage?.CanFreeze ?? false)
			{
				this.PreviewImage.Freeze();
			}
		}

		public static UploadMedia FromBitmapSource(BitmapSource bmpSource, string ext = "CLIP")
		{
			return new UploadMedia(bmpSource, MediaType.Image, ext);
		}

		public static UploadMedia FromArtwork(ArtworkItem artwork)
		{
			return new UploadMedia(artwork.Image, MediaType.Image, "ARTW");
		}

		public static UploadMedia FromFile(string filepath)
		{
			return new UploadMedia(filepath);
		}

		private static bool IsAnimatedGif(string filename) => ImageAnimator.CanAnimate(Image.FromFile(filename));

		public string FileName { get; private set; }

		public string FilePath { get; private set; }

		public MediaType MediaType { get; }

		public string ViewExtension { get; }

		public BitmapImage PreviewImage { get; private set; }

		public long? UploadId { get; private set; }

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

		private bool _isUploadFailed;
		public bool IsUploadFailed
		{
			get => this._isUploadFailed;
			private set => this.SetProperty(ref this._isUploadFailed, value);
		}

		private bool _isUploading;
		public bool IsUploading
		{
			get => this._isUploading;
			private set => this.SetProperty(ref this._isUploading, value);
		}

		private bool _isTweetPosting;
		public bool IsTweetPosting => this._isTweetPosting;

		public bool UseChunkedUpload { get; }

		public Stream SourceStream { get; private set; }

		private void CleanUploadState()
		{
			this.IsUploading = false;
			this.UploadProgress = 0.0d;
			this.IsUploadFailed = false;
		}

		public void SetIsTweetPosting(bool value)
		{
			SetProperty(ref this._isTweetPosting, value, nameof(IsTweetPosting));
		}

		public async Task Upload(TwitterApi tokens)
		{
			bool isVideoUpload = (MediaType & MediaType.Video) != 0;

			var uploadType = isVideoUpload
				? MimeTypes.Video.Mp4
				: MimeTypes.OctetStream;

			this.CleanUploadState();

			this.IsUploading = true;

			this.SourceStream.Position = 0;

			try
			{
				Task<MediaResponse> task;

				if (isVideoUpload)
				{
					task = tokens.Media.ChunkedUpload(
						media: SourceStream,
						mediaType: uploadType,
						progressReceiver: this);
				}
				else
				{
					task = tokens.Media.Upload(SourceStream);
				}

				var result = await task.ConfigureAwait(true);

				this.UploadId = result.MediaId;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				this.IsUploadFailed = true;
			}
			finally
			{
				this.IsUploading = false;
			}
		}

		public bool IsAvailableUploadId() => UploadId.HasValue && UploadId > 0;

		public void Report(UploadProgress value)
		{
			this.UploadProgress = value.UploadPercentage;
		}

		public void Dispose()
		{
			if (SourceStream != null)
			{
				SourceStream.Dispose();
				SourceStream = null;
			}

			if (PreviewImage != null)
			{
				PreviewImage = null;
			}

			FilePath = null;
			FileName = null;
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
