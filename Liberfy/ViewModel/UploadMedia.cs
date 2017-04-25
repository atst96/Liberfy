using CoreTweet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static Liberfy.Defines;

namespace Liberfy.ViewModel
{
	internal class UploadMedia : NotificationObject, IProgress<UploadChunkedProgressInfo>, IDisposable
	{
		public UploadMedia(BitmapSource bmpSource)
		{
			var encoder = new PngBitmapEncoder();
			SourceStream = new MemoryStream();
			PreviewImage = new BitmapImage();

			encoder.Frames.Add(BitmapFrame.Create(bmpSource));
			encoder.Save(SourceStream);
			encoder = null;

			SourceStream.Position = 0;
			PreviewImage.BeginInit();
			PreviewImage.StreamSource = SourceStream;
			PreviewImage.EndInit();

			MediaType = MediaType.Image;
			ViewExtension = "CLIP";
		}

		public UploadMedia(ArtworkItem artwork)
		{
			ViewExtension = "ARTW";
			MediaType = MediaType.Image;
			PreviewImage = artwork.Image;
			SourceStream = artwork.Image.StreamSource;
		}

		public UploadMedia(string filePath)
		{
			var ext = Path.GetExtension(filePath).ToLower();

			if (ext.Equals(".gif") && IsAnimatedGif(ext))
			{
				MediaType = MediaType.AnimatedGifFile;
				PreviewImage = new BitmapImage(new Uri(filePath));
			}
			else if (VideoExtensions.Contains(ext))
			{
				MediaType = MediaType.VideoFile;
			}
			else if (ImageExtensions.Contains(ext))
			{
				MediaType = MediaType.ImageFile;

				if (ext != ".webp")
				{
					PreviewImage = new BitmapImage(new Uri(filePath));
				}
			}
			else
			{
				throw new NotSupportedException();
			}

			FilePath = filePath;
			filePath = Path.GetFileName(filePath);
			ViewExtension = ext.Substring(1).ToUpper();
			SourceStream = File.OpenRead(FilePath);

			if (PreviewImage?.CanFreeze ?? false)
			{
				PreviewImage.Freeze();
			}
		}

		private static bool IsAnimatedGif(string filename)
		{
			return System.Drawing.ImageAnimator.CanAnimate(
				System.Drawing.Image.FromFile(filename));
		}

		public string FileName { get; private set; }

		public string FilePath { get; private set; }

		public MediaType MediaType { get; }

		public string ViewExtension { get; }

		public BitmapImage PreviewImage { get; private set; }

		private string _description;
		public string Description
		{
			get => _description;
			set => SetProperty(ref _description, value);
		}

		private double _uploadProgress;
		public double UploadProgress
		{
			get => _uploadProgress;
			set => SetProperty(ref _uploadProgress, value);
		}

		private bool _isUploadFailed;
		public bool IsUploadFailed
		{
			get => _isUploadFailed;
			set => SetProperty(ref _isUploadFailed, value);
		}

		private void ClearState()
		{
			IsUploading = false;
			UploadProgress = 0.0d;
			IsUploadFailed = false;
		}

		public Stream SourceStream { get; private set; }

		private bool _isUploading;
		public bool IsUploading
		{
			get => _isUploading;
			set => SetProperty(ref _isUploading, value);
		}

		public Task<MediaUploadResult> Upload(Account account)
		{
			UploadMediaType uploadType;

			if(MediaType.HasFlag(MediaType.Video))
			{
				uploadType = UploadMediaType.Image;
			}
			else
			{
				uploadType = UploadMediaType.Image;
			}

			ClearState();
			_cancellationTokenSource = new CancellationTokenSource();

			IsUploading = true;

			return account.Tokens.Media
				.UploadChunkedAsync(SourceStream, uploadType, null, _cancellationTokenSource.Token, this)
				.ContinueWith(OnUploadCompalted, TaskScheduler.FromCurrentSynchronizationContext());
		}

		public void Report(UploadChunkedProgressInfo report)
		{
			UploadProgress = report.ProcessingProgressPercent;
		}

		public MediaUploadResult OnUploadCompalted(Task<MediaUploadResult> task)
		{
			var result = task.Result;

			IsUploading = false;

			return result;
		}

		CancellationTokenSource _cancellationTokenSource;

		public void CancelUpload()
		{
			if (!_cancellationTokenSource?.IsCancellationRequested ?? true)
			{
				_cancellationTokenSource.Cancel();
			}
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
	enum MediaType : uint
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
