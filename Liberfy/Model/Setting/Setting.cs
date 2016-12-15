using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Converters;
using System.Windows;
using System.IO;

namespace Liberfy
{
	[JsonObject]
	internal partial class Setting : NotificationObject
	{
		#region

		[JsonIgnore]
		public Application app = Application.Current;

		private void SetValue(string key, object value)
		{
			app.Resources[key] = value;
		}

		private T GetValue<T>(string key)
		{
			return (T)app.TryFindResource(key);
		}

		#endregion

		#region Generic

		private bool _checkUpdate = true;
		public DateTime LastUpdateChecked { get; set; }
		public bool MinimizeStartup { get; set; } = true;
		public bool StayingInTaskTray { get; set; } = false;
		public bool AlwaysShowInTaskbar { get; set; } = true;
		private BackgroundType _backgroundType = BackgroundType.None;
		private AlignmentX _imageAlignmentX = AlignmentX.Left;
		private AlignmentY _imageAlignmentY = AlignmentY.Top;
		private Stretch _imageStretch = Stretch.UniformToFill;
		private double _imageOpacity = 1.0d;
		private string _imagePath = string.Empty;


		public bool SupressShutdown { get; set; } = false;

		public bool CheckUpdate
		{
			get { return _checkUpdate; }
			set { SetProperty(ref _checkUpdate, value); }
		}

		public BackgroundType BackgroundType
		{
			get { return _backgroundType; }
			set { SetProperty(ref _backgroundType, value); }
		}

		public AlignmentX ImageAlignmentX
		{
			get { return _imageAlignmentX; }
			set { SetProperty(ref _imageAlignmentX, value); }
		}

		public AlignmentY ImageAlignmentY
		{
			get { return _imageAlignmentY; }
			set { SetProperty(ref _imageAlignmentY, value); }
		}

		public Stretch ImageStretch
		{
			get { return _imageStretch; }
			set { SetProperty(ref _imageStretch, value); }
		}

		public double ImageOpacity
		{
			get { return _imageOpacity; }
			set { SetProperty(ref _imageOpacity, value); }
		}

		public string ImagePath
		{
			get { return _imagePath; }
			set { SetProperty(ref _imagePath, value); }
		}

		public uint StartupWindowMode { get; set; } = 0;

		#endregion

		#region View settings

		public const string DefaultTimelineFont = "Meiryo, Segoe UI Symbol";
		public const double DefaultTimelineFontSize = 12;

		public const string DefaultUIFont = "Meiryo";
		public const double DefaultUIFontSize = 12;

		private bool _timelineUseUIFont = true;
		private string _timelineFont = DefaultTimelineFont;
		private double _timelineFontSize = DefaultTimelineFontSize;
		private bool _uiUseSystemFont;
		private string _uiFont = DefaultUIFont;
		private double _uiFontSize = DefaultUIFontSize;

		public bool TimelineUseUIFont
		{
			get { return _timelineUseUIFont; }
			set
			{
				if (SetProperty(ref _timelineUseUIFont, value))
				{
					RaisePropertyChanged(nameof(TimelineFont));
					RaisePropertyChanged(nameof(TimelineFontSize));
				}
			}
		}

		public string TimelineFont
		{
			get
			{
				return _timelineUseUIFont
				  ? UIFont : string.IsNullOrEmpty(_timelineFont)
				  ? _timelineFont : _timelineFont = DefaultTimelineFont;
			}
			set { SetProperty(ref _timelineFont, value); }
		}

		public double TimelineFontSize
		{
			get { return _timelineUseUIFont ? UIFontSize : _timelineFontSize; }
			set { SetProperty(ref _timelineFontSize, value); }
		}

		public bool UIUseSystemFont
		{
			get { return _uiUseSystemFont; }
			set
			{
				if (SetProperty(ref _uiUseSystemFont, value))
				{
					RaisePropertyChanged(nameof(UIFont));
					RaisePropertyChanged(nameof(UIFontSize));
					RaisePropertyChanged(nameof(TimelineFont));
					RaisePropertyChanged(nameof(TimelineFontSize));
				}
			}
		}

		public string UIFont
		{
			get
			{
				return _uiUseSystemFont
				  ? "" : !string.IsNullOrEmpty(_uiFont)
				  ? _uiFont : _uiFont = DefaultUIFont;
			}
			set { SetProperty(ref _uiFont, value); }
		}

		public double UIFontSize
		{
			get { return _uiUseSystemFont ? double.NaN : _uiFontSize; }
			set { SetProperty(ref _uiFontSize, value); }
		}

		public bool ShowImageInTimeline
		{
			get { return GetValue<bool>("ShowImageInTimeline"); }
			set
			{
				SetValue("ShowImageInTimeline", value);
				RaisePropertyChanged(nameof(ShowImageInTimeline));
			}
		}

		public bool ShowImageInDetail
		{
			get { return GetValue<bool>("ShowImageInDetail"); }
			set
			{
				SetValue("ShowImageInDetail", value);
				RaisePropertyChanged("ShowImageInDetail");
			}
		}

		public bool RelativeTime
		{
			get { return GetValue<bool>("RelativeTime"); }
			set { SetValue("RelativeTime", value); }
		}

		public TextFormattingMode TextFormattingMode
		{
			get { return GetValue<TextFormattingMode>("TextFormattingMode"); }
			set
			{
				SetValue("TextFormattingMode", value);
				RaisePropertyChanged("TextFormattingMode");
			}
		}

		public bool EnableTimelineAnimation { get; set; } = true;

		public bool DisableAnimationAtTerminalConnection { get; set; } = false;

		public bool ShowActionButtonInTimeline
		{
			get { return GetValue<bool>("ShowActionButtonInTimeline"); }
			set { SetValue("ShowActionButtonInTimeline", value); }
		}

		#endregion

		#region Format settings

		const string DefaultNowPlayingFormat = @"%artist% - %name% / %album% #NowPlaying";

		public string NowPlayingFormat { get; set; }

		private bool _insertThumbnailAtNowPlaying;

		public bool InsertThumbnailAtNowPlayying
		{
			get { return _insertThumbnailAtNowPlaying; }
			set { SetProperty(ref _insertThumbnailAtNowPlaying, value); }
		}

		#endregion

		#region Notification settings

		private const string _defSoundPath = @"%windir%\Media\Windows Notify.wav";
		private static string @DefaultSoundFile => Environment.ExpandEnvironmentVariables(_defSoundPath);

		private bool _enableNotification = true;
		public bool EnableNotification
		{
			get { return _enableNotification; }
			set { SetProperty(ref _enableNotification, value); }
		}

		private string _notificationSoundFile;
		public string NotificationSoundFile
		{
			get { return _notificationSoundFile ?? (_notificationSoundFile = DefaultNowPlayingFormat); }
			set { SetProperty(ref _notificationSoundFile, value); }
		}

		private bool _enableSoundNotification;
		public bool EnableSoundNotification
		{
			get { return _enableSoundNotification; }
			set { SetProperty(ref _enableSoundNotification, value); }
		}

		private bool _enableBalloonNotification;
		public bool EnableBalloonNotification
		{
			get { return _enableBalloonNotification; }
			set { SetProperty(ref _enableBalloonNotification, value); }
		}

		[JsonIgnore]
		public DictionaryEx<NotifyCode, bool> NotificationEvents { get; } = new DictionaryEx<NotifyCode, bool>();

		public bool Notification_Reply
		{
			get { return NotificationEvents.GetOrAdd(NotifyCode.Reply, true); }
			set { NotificationEvents[NotifyCode.Reply] = value; }
		}

		public bool Notification_Favorite
		{
			get { return NotificationEvents.GetOrAdd(NotifyCode.Favorite, true); }
			set { NotificationEvents[NotifyCode.Favorite] = value; }
		}

		public bool Notification_QuotedTweet
		{
			get { return NotificationEvents.GetOrAdd(NotifyCode.QuotedTweet, true); }
			set { NotificationEvents[NotifyCode.QuotedTweet] = value; }
		}

		public bool Notification_Retweet
		{
			get { return NotificationEvents.GetOrAdd(NotifyCode.Retweet, true); }
			set { NotificationEvents[NotifyCode.Retweet] = value; }
		}

		public bool Notification_RetweetedRetweet
		{
			get { return NotificationEvents.GetOrAdd(NotifyCode.RetweetedRetweet, true); }
			set { NotificationEvents[NotifyCode.RetweetedRetweet] = value; }
		}

		public bool Notification_FavoritedRetweet
		{
			get { return NotificationEvents.GetOrAdd(NotifyCode.FavoritedRetweet, true); }
			set { NotificationEvents[NotifyCode.FavoritedRetweet] = value; }
		}

		public bool Notification_ListMemberAdded
		{
			get { return NotificationEvents.GetOrAdd(NotifyCode.ListMemberAdded, true); }
			set { NotificationEvents[NotifyCode.ListMemberAdded] = value; }
		}

		public bool Notification_Follow
		{
			get { return NotificationEvents.GetOrAdd(NotifyCode.Follow, true); }
			set { NotificationEvents[NotifyCode.Follow] = value; }
		}

		public bool Notification_DirectMessageReceived
		{
			get { return NotificationEvents.GetOrAdd(NotifyCode.DirectMessageCreated, true); }
			set { NotificationEvents[NotifyCode.DirectMessageCreated] = value; }
		}

		#endregion

		#region Mute settings

		private FluidCollection<Mute> _mute;
		public FluidCollection<Mute> Mute => _mute ?? (_mute = new FluidCollection<Mute>());

		#endregion

		#region Post settings

		public bool CloseWindowAfterPostComplated { get; set; }

		#endregion

		#region Network settings

		public bool UseSystemProxy { get; set; }

		#endregion
	}

	enum BackgroundType
	{
		[JsonProperty("none")]
		None,

		[JsonProperty("color")]
		Color,

		[JsonProperty("picture")]
		Picture,
	}

	enum NotifyCode
	{
		[JsonProperty("reply")]
		Reply,

		[JsonProperty("retweet")]
		Retweet,

		[JsonProperty("direct_message_created")]
		DirectMessageCreated,

		[JsonProperty("direct_message_deleted")]
		DirectMessageDeleted,

		[JsonProperty("block")]
		Block,

		[JsonProperty("unblock")]
		Unblock,

		[JsonProperty("favorite")]
		Favorite,

		[JsonProperty("unfavorite")]
		Unfavorite,

		[JsonProperty("follow")]
		Follow,

		[JsonProperty("unfollow")]
		Unfollow,

		[JsonProperty("list_created")]
		ListCreated,

		[JsonProperty("list_destroyed")]
		ListDestroyed,

		[JsonProperty("list_updated")]
		ListUpdated,

		[JsonProperty("list_member_added")]
		ListMemberAdded,

		[JsonProperty("list_member_removed")]
		ListMemberRemoved,

		[JsonProperty("list_user_subscribed")]
		ListUserSubscribed,

		[JsonProperty("list_user_unsubscribed")]
		ListUserUnsubscribed,

		[JsonProperty("user_update")]
		UserUpdate,

		[JsonProperty("mute")]
		Mute,

		[JsonProperty("unmute")]
		Unmute,

		[JsonProperty("favorited_retweet")]
		FavoritedRetweet,

		[JsonProperty("retweeted_retweet")]
		RetweetedRetweet,

		[JsonProperty("quoted_tweet")]
		QuotedTweet
	}
}
