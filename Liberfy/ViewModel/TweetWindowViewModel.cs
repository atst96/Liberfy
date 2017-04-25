using NowPlayingLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Twitter.Text;
using static Liberfy.Defines;

namespace Liberfy.ViewModel
{
	class TweetWindow : ViewModelBase
	{
		private static readonly Validator tweetValidator = new Validator();
		protected static Setting Setting => App.Setting;

		private const int MaxTweetLength = 140;
		private const int MediaUrlLength = 23;

		public TweetWindow()
		{
			Media = new FluidCollection<UploadMedia>();

			UpdateCanPost();
		}

		public FluidCollection<UploadMedia> Media { get; }

		private bool _isPosting = false;

		private bool _canUpdateContent;
		public bool CanUpdateContent => _canUpdateContent;

		private int _remainingTweetLength = MaxTweetLength;
		public int RemainingTweetLength => _remainingTweetLength;

		private string _tweet = string.Empty;
		public string Tweet
		{
			get => _tweet;
			set
			{
				var newTweet = value?.Replace("\r\n", "\n");
				if (SetProperty(ref _tweet, newTweet))
				{
					UpdateCanPost();
				}
			}
		}

		private void UpdateCanPost()
		{
			// ツイート可能な残り文字数の算出
			int tweetLength = tweetValidator.GetTweetLength(_tweet);
			int actualTweetLength = tweetLength;
			if (Media.Count > 0)
			{
				actualTweetLength += MediaUrlLength;
			}

			_remainingTweetLength = MaxTweetLength - actualTweetLength;

			// アップロード可能かを判定
			bool isTweetValiableLength =
				_remainingTweetLength > 0 && _remainingTweetLength < MaxTweetLength;
			bool isPostableMediaCount = Media.Count <= 4;

			_canUpdateContent = isTweetValiableLength && isPostableMediaCount;

			// プロパティの変更通知
			RaisePropertyChanged(nameof(RemainingTweetLength));
			RaisePropertyChanged(nameof(CanUpdateContent));

			PostCommand.RaiseCanExecute();
		}

		#region PostCommand

		private Command _postCommand;
		public Command PostCommand => _postCommand
			?? (_postCommand = RegisterReleasableCommand(PostTweet, CanPostTweet));

		public void PostTweet()
		{
			OnPostBegin();
			OnPostEnd();
		}

		public bool CanPostTweet(object _) => !_isPosting && IsEditable && _canUpdateContent;

		public bool IsEditable { get; private set; } = true;

		private void OnPostBegin()
		{
			_isPosting = true;
			SetIsEditable(false);
		}

		private void OnPostEnd()
		{
			_isPosting = false;
			SetIsEditable(true);
		}

		private void SetIsEditable(bool canEdit)
		{
			IsEditable = canEdit;
			RaisePropertyChanged(nameof(IsEditable));

			AddImageCommand.RaiseCanExecute();
			PostCommand.RaiseCanExecute();
		}

		#endregion

		#region AddImageCommand

		private Command _addImageCommand;
		public Command AddImageCommand => _addImageCommand
			?? (_addImageCommand = RegisterReleasableCommand(AddImage, CanEditContent));

		void AddImage()
		{
			var ofd = new Microsoft.Win32.OpenFileDialog
			{
				Title = "アップロードするメディアを選択",
				Filter = UploadableExtensionFilter,
				DereferenceLinks = true,
			};

			if (DialogService.OpenModal(ofd))
			{
				var extension = Path.GetExtension(ofd.FileName);

				if (IsUploadableExtension(extension)
					|| DialogService.ShowQuestion("指定されたファイルはアップロードに失敗する可能性があります。\n続行しますか？"))
				{
					Media.Add(new UploadMedia(ofd.FileName));
					UpdateCanPost();
				}
			}

			ofd.Reset();
		}

		bool IsUploadableExtension(string ext)
		{
			return UploadableMediaExtensions.Contains(ext.ToLower());
		}

		bool CanEditContent(object o) => !_isPosting;



		private static string CreateExtensionFilter()
		{
			// OpenFileDialogで用いる拡張子フィルタの生成
			// e.g. 表示名|*.ext1|表示名(拡張子複数指定)|*.ext2;*.ext2|...

			var medExts = $"アップロード可能なメディア|*{string.Join(";*", UploadableMediaExtensions)}";
			var imgExts = $"画像ファイル|*{string.Join(";*", ImageExtensions)}";
			var vidExts = $"動画ファイル|*{string.Join(";*", VideoExtensions)}";
			var allExts = "すべてのファイル|*.*";

			// 上記の文字列を‘|’(縦線)を区切り文字として結合
			return string.Join("|", medExts, imgExts, vidExts, allExts);
		}

		private static readonly string UploadableExtensionFilter = CreateExtensionFilter();

		#endregion

		#region RemoveImageCommand

		private Command<UploadMedia> _removeMediaCommand;
		public Command<UploadMedia> RemoveMediaCommand => _removeMediaCommand
			?? (_removeMediaCommand = RegisterReleasableCommand<UploadMedia>(RemoveMedia, CanRemoveMedia));

		private bool CanRemoveMedia(UploadMedia media)
		{
			return Media.Contains(media);
		}

		private void RemoveMedia(UploadMedia media)
		{
			using (media)
			{
				Media.Remove(media);
				UpdateCanPost();
			}
		}

		#endregion

		private TextBoxController _textBoxController = new TextBoxController();
		public TextBoxController TextBoxController => _textBoxController;

		#region DragDropCommand

		private Command<IDataObject> _dragDropCommand;
		public Command<IDataObject> DragDropCommand => _dragDropCommand
			?? (_dragDropCommand = RegisterReleasableCommand<IDataObject>(Dropped, CanDrop));

		private DragDropEffects _dragDropEffects = DragDropEffects.None;
		public DragDropEffects DragDropEffects
		{
			get => _dragDropEffects;
			set => SetProperty(ref _dragDropEffects, value);
		}

		private string _dropDescriptionMessage;
		public string DropDescriptionMessage
		{
			get => _dropDescriptionMessage;
			set => SetProperty(ref _dropDescriptionMessage, value);
		}

		private DropImageType _dropDescriptionIcon;
		public DropImageType DropDescriptionIcon
		{
			get => _dropDescriptionIcon;
			set => SetProperty(ref _dropDescriptionIcon, value);
		}

		private bool CanDrop(IDataObject data)
		{
			if (!IsEditable) return false;

			if (data.GetDataPresent(DataFormats.FileDrop)
				&& data.GetData(DataFormats.FileDrop) is string[] dropppedFiles
				&& !dropppedFiles.Any(f => !IsUploadableExtension(Path.GetExtension(f))))
			{
				DropDescriptionMessage = "添付";
				DragDropEffects = DragDropEffects.Copy;
				DropDescriptionIcon = DropImageType.Copy;
			}
			else if (UrlDataPresets.Any(data.GetDataPresent)
				|| data.GetDataPresent(DataFormats.UnicodeText)
				|| data.GetDataPresent(DataFormats.Text))
			{
				DropDescriptionMessage = "挿入";
				DragDropEffects = DragDropEffects.Copy;
				DropDescriptionIcon = DropImageType.Label;
			}
			else
			{
				DropDescriptionMessage = "無効な形式";
				DragDropEffects = DragDropEffects.None;
				DropDescriptionIcon = DropImageType.None;
				return false;
			}

			return true;
		}

		private void Dropped(IDataObject data)
		{
			System.Diagnostics.Debug.WriteLine(string.Join(", ", data.GetFormats()));

			if (data.GetDataPresent(DataFormats.FileDrop))
			{
				var droppedFiles = (string[])data.GetData(DataFormats.FileDrop);
				Media.AddRange(droppedFiles.Select(f => new UploadMedia(f)));
				UpdateCanPost();
			}
			else if (UrlDataPresets.Any(data.GetDataPresent))
			{
				TextBoxController.Insert((string)data.GetData(DataFormats.UnicodeText));
				TextBoxController.Focus();
			}
			else if (data.GetDataPresent(DataFormats.UnicodeText))
			{
				TextBoxController.Insert((string)data.GetData(DataFormats.UnicodeText));
				TextBoxController.Focus();
			}
			else if (data.GetDataPresent(DataFormats.Text))
			{
				TextBoxController.Insert((string)data.GetData(DataFormats.Text));
				TextBoxController.Focus();
			}
		}

		private readonly static string[] UrlDataPresets = { "IESiteModeToUrl", "text/x-moz-url", "UniformResourceLocator" };

		#endregion

		#region NowPlaying

		private string _nowPlayingPlayer = Setting.NowPlayingDefaultPlayer;
		public string NowPlayingPlayer
		{
			get => _nowPlayingPlayer;
			set => SetProperty(ref _nowPlayingPlayer, value, _getNowPlayingTextCommand);
		}

		private string _insertinNowPlayingText;
		public string InsertionNowPlayingText
		{
			get => _insertinNowPlayingText;
			set => SetProperty(ref _insertinNowPlayingText, value);
		}

		private bool _isNowPlayingPanelOpen;
		public bool IsNowPlayingPanelOpen
		{
			get => _isNowPlayingPanelOpen;
			set => SetProperty(ref _isNowPlayingPanelOpen, value);
		}

		public FluidCollection<ArtworkItem> NowPlayingArtworks { get; } = new FluidCollection<ArtworkItem>();

		#region Command: GetNowPlayingTextCommand

		private Command _getNowPlayingTextCommand;
		public Command GetNowPlayingTextCommand => _getNowPlayingTextCommand
			?? (_getNowPlayingTextCommand = RegisterReleasableCommand(GetNowPlayingText, IsSupportedPlayer));

		private bool IsSupportedPlayer(object o)
		{
			return !string.IsNullOrEmpty(_nowPlayingPlayer)
				&& NowPlayingPlayerList.ContainsKey(_nowPlayingPlayer);
		}

		private async void GetNowPlayingText()
		{
			var atwks = NowPlayingArtworks.ToArray();
			NowPlayingArtworks.Clear();
			atwks.DisposeAll();

			MediaPlayerBase player = null;
			bool isProcessRunning = Process.GetProcessesByName(_nowPlayingPlayer).Length > 0;

			if (!isProcessRunning)
			{
				DialogService.MessageBox(
					$"再生情報の取得に失敗しました。プレーヤが起動しているか確認してください。",
					MsgBoxButtons.Ok, MsgBoxIcon.Error);
				return;
			}

			try
			{
				switch (_nowPlayingPlayer)
				{
					case "wmplayer":
						player = new WindowsMediaPlayer();
						break;

					case "itunes":
						player = new iTunes();
						break;

					case "foobar2000":
						player = new NowPlayingLib.Foobar2000();
						break;

					default:
						return;
				}

				var media = await player.GetCurrentMedia();

				InsertionNowPlayingText = ReplaceMediaAlias(media, Setting.NowPlayingFormat);
				foreach (var stream in media.Artworks)
				{
					try
					{
						NowPlayingArtworks.Add(new ArtworkItem(stream, Setting.InsertThumbnailAtNowPlayying));
					}
					catch { /* 非対応形式のアートワークは処理しない */ }
				}
			}
			catch (Exception ex)
			{
				DialogService.MessageBox(
					$"再生情報の取得に失敗しました。プレーヤで楽曲が再生中かどうか確認してください。\n\nエラー：\n{ex.Message}",
					MsgBoxButtons.Ok, MsgBoxIcon.Error);
			}
			finally
			{
				if (player != null)
				{
					player.Dispose();
					player = null;
				}
			}
		}

		public static IReadOnlyDictionary<string, string> NowPlayingPlayerList { get; }
		= new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
		{
			["wmplayer"] = "Windows Media Player",
			["itunes"] = "iTunes",
			["foobar2000"] = "foobar2000"
		});

		private static bool IsProcessRunning(string processName)
		{
			return System.Diagnostics.Process.GetProcessesByName(processName).Any();
		}

		private string ReplaceMediaAlias(MediaItem media, string format)
		{
			var aliases = new Dictionary<string, string>
			{
				["album"] = media.Album,
				["album_artist"] = media.AlbumArtist,
				["artist"] = media.Artist,
				["composer"] = media.Composer,
				["category"] = media.Category,
				["genre"] = media.Genre,
				["name"] = media.Name,
				["number"] = media.TrackNumber.ToString(),
				["year"] = media.Year.ToString()
			};

			string replacedString = format;

			foreach (var arias in aliases)
			{
				replacedString = Regex.Replace(
					replacedString, $"%{arias.Key}%", arias.Value, RegexOptions.IgnoreCase);
			}

			return replacedString;
		}

		#endregion Command: GetNowPlayingTextCommand

		#region Command: InsertNowPlayingTextCommand

		private Command _insertNowPlayingTextCommand;
		public Command InsertNowPlayingTextCommand => _insertNowPlayingTextCommand
			?? (_insertNowPlayingTextCommand = new DelegateCommand(InsertNowPlaying));

		private void InsertNowPlaying()
		{
			TextBoxController.Insert(InsertionNowPlayingText);

			foreach (var artwork in NowPlayingArtworks)
			{
				if (artwork.Use)
				{
					Media.Add(new UploadMedia(artwork));
					artwork.Dispose(false);
				}
				else
				{
					artwork.Dispose(true);
				}
			}

			NowPlayingArtworks.Clear();

			InsertionNowPlayingText = null;
			IsNowPlayingPanelOpen = false;
		}

		#endregion Command: InsertNowPlayingTextCommand

		#endregion NowPlaying

		#region ImagePasting

		private Command _pasteImageCommand;
		public Command PasteImageCommand => _pasteImageCommand
			?? (_pasteImageCommand = new DelegateCommand(OnImagePasted, CanImagePaste));

		private bool CanImagePaste(object obj)
		{
			return Clipboard.ContainsImage();
		}

		private void OnImagePasted()
		{
			if (Clipboard.ContainsImage())
			{
				Media.Add(new UploadMedia(Clipboard.GetImage()));
			}
		}

		#endregion

		internal override void OnClosed()
		{
			_textBoxController = null;

			foreach (var media in Media)
			{
				media.Dispose();
			}

			base.OnClosed();
		}
	}


	internal class ArtworkItem : NotificationObject, IDisposable
	{
		public ArtworkItem(Stream stream, bool? use)
		{
			_image = new BitmapImage();
			_image.BeginInit();
			_image.StreamSource = stream;
			_image.EndInit();

			_artStream = stream;

			this._use = use ?? false;
		}

		private bool _use;
		public bool Use
		{
			get => _use;
			set => SetProperty(ref _use, value);
		}

		private BitmapImage _image;
		public BitmapImage Image => _image;

		private Stream _artStream;
		public Stream ArtStream => _artStream;

		public void Dispose()
		{
			Dispose(true);
		}

		public void Dispose(bool disposeStream)
		{
			if (disposeStream)
			{
				_artStream?.Dispose();
			}

			_image = null;
			_artStream = null;
		}
	}
}