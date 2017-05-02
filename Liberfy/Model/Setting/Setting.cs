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
using System.Runtime.Serialization;
using static Liberfy.Defines;

namespace Liberfy
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	internal class Setting : NotificationObject
	{
		#region Generic

		[JsonProperty("startup_check_update")]
		private bool _checkUpdate = true;
		public bool CheckUpdate
		{
			get => _checkUpdate;
			set => SetProperty(ref _checkUpdate, value);
		}

		[JsonProperty("startup_minimized")]
		private bool _minimizeStartup;
		public bool MinimizeStartup
		{
			get => _minimizeStartup;
			set => SetProperty(ref _minimizeStartup, value);
		}

		[JsonProperty("tasktray_show")]
		private bool _showInTaskTray;
		public bool ShowInTaskTray
		{
			get => _showInTaskTray;
			set => SetProperty(ref _showInTaskTray, value);
		}

		[JsonProperty("tasktray_show_at_minimized")]
		private bool _showInTaskTrayAtMinimized = true;
		public bool ShowInTaskTrayAtMinimzied
		{
			get => _showInTaskTrayAtMinimized;
			set => SetProperty(ref _showInTaskTrayAtMinimized, value);
		}

		[JsonProperty("tasktray_balloon_at_minimized")]
		private bool _showBalloonAtMinimized = true;
		public bool ShowBalloonAtMinimized
		{
			get => _showBalloonAtMinimized;
			set => SetProperty(ref _showBalloonAtMinimized, value);
		}

		[JsonProperty("minimize_click_close_button")]
		private bool _minimizeAtCloseButtonClick;
		public bool MinimizeAtCloseButtonClick
		{
			get => _minimizeAtCloseButtonClick;
			set => SetProperty(ref _minimizeAtCloseButtonClick, value);
		}


		private bool _suppressShutdown;
		[JsonProperty("system_suppress_shutdown")]
		public bool SuppressShutdown
		{
			get => _suppressShutdown;
			set => SetProperty(ref _suppressShutdown, value);
		}

		[JsonProperty("system_suppress_suspend")]
		private bool _suppressSuspend;
		public bool SuppressSuspend
		{
			get => _suppressSuspend;
			set => SetProperty(ref _suppressSuspend, value);
		}

		[JsonProperty("system_suppress_screensaver")]
		private bool _suppressScreenSaver;
		public bool SuppressScreenSaver
		{
			get => _suppressScreenSaver;
			set => SetProperty(ref _suppressScreenSaver, value);
		}

		[JsonProperty("background_type")]
		private BackgroundType _backgroundType = BackgroundType.None;
		public BackgroundType BackgroundType
		{
			get => _backgroundType;
			set => SetProperty(ref _backgroundType, value);
		}

		[JsonProperty("background_alignment_x")]
		private AlignmentX _imageAlignmentX;
		public AlignmentX ImageAlignmentX
		{
			get => _imageAlignmentX;
			set => SetProperty(ref _imageAlignmentX, value);
		}

		[JsonProperty("background_alignment_y")]
		private AlignmentY _imageAlignmentY = AlignmentY.Top;
		public AlignmentY ImageAlignmentY
		{
			get => _imageAlignmentY;
			set => SetProperty(ref _imageAlignmentY, value);
		}

		[JsonProperty("background_image_stretch")]
		private Stretch _backgroundImageStretch = Stretch.UniformToFill;
		public Stretch BackgroundImageStretch
		{
			get => _backgroundImageStretch;
			set => SetProperty(ref _backgroundImageStretch, value);
		}

		[JsonProperty("background_image_opacity")]
		private double _backgroundOpacity = 1.0d;
		public double BackgroundImageOpacity
		{
			get => _backgroundOpacity;
			set => SetProperty(ref _backgroundOpacity, value);
		}

		[JsonProperty("background_image_path")]
		private string _backgroundImagePath;
		public string BackgroundImagePath
		{
			get => _backgroundImagePath;
			set => SetProperty(ref _backgroundImagePath, value);
		}

		#endregion

		#region Account

		[JsonProperty("account_column_defaults")]
		private FluidCollection<ColumnSetting> _defaultColumns;
		public FluidCollection<ColumnSetting> DefaultColumns
		{
			get
			{
				if (_defaultColumns != null)
				{
					return _defaultColumns;
				}
				else
				{
					var account = Account.Dummy;

					return _defaultColumns =
						new FluidCollection<ColumnSetting>(new[] {
							new ColumnSetting(ColumnType.Home, account),
							new ColumnSetting(ColumnType.Notification, account),
							new ColumnSetting(ColumnType.Messages, account)
						});
				}
			}
		}

		#endregion

		#region View

		[JsonProperty("timeline_fonts")]
		private string[] _timelineFont;
		public string[] TimelineFont
		{
			get => _timelineFont ?? (_timelineFont = DefaultTimelineFont);
			set => SetProperty(ref _timelineFont, value);
		}

		[JsonProperty("timeline_font_size")]
		private double _timelineFontSize;
		public double TimelineFontSize
		{
			get => _timelineFontSize;
			set => SetProperty(ref _timelineFontSize, value);
		}


		[JsonProperty("timeline_tweet_show_media")]
		private bool _timelineStatusShowMedia = true;
		public bool TimelineStatusShowMedia
		{
			get => _timelineStatusShowMedia;
			set => SetProperty(ref _timelineStatusShowMedia, value);
		}

		[JsonProperty("timeline_tweet_show_media_detail")]
		private bool _timelineStatusDetailShowMedia = true;
		public bool TimelineStatusDetailShowMedia
		{
			get => _timelineStatusDetailShowMedia;
			set => SetProperty(ref _timelineStatusDetailShowMedia, value);
		}

		[JsonProperty("timeline_tweet_show_quoted_tweet")]
		private bool _timelineStatusShowQuotedTweet = true;
		public bool TimelineStatusShowQuotedTweet
		{
			get => _timelineStatusDetailShowMedia;
			set => SetProperty(ref _timelineStatusShowQuotedTweet, value);
		}

		[JsonProperty("timeline_tweet_show_quoted_tweet_detail")]
		private bool _timelineStatusDetailShowQuotedTweet = true;
		public bool TimelineStatusDetailShowQuotedTweet
		{
			get => _timelineStatusDetailShowQuotedTweet;
			set => SetProperty(ref _timelineStatusDetailShowQuotedTweet, value);
		}

		[JsonProperty("timeline_tweet_show_relative_time")]
		private bool _timelineStatusShowRelativeTime = true;
		public bool TimelineStatusShowRelativeTime
		{
			get => _timelineStatusShowRelativeTime;
			set => SetProperty(ref _timelineStatusShowRelativeTime, value);
		}

		[JsonProperty("timeline_tweet_show_relatvie_time_detail")]
		private bool _timelineStatusDetailShowRelativeTime;
		public bool TimelineStatusDetailShowRelativeTime
		{
			get => _timelineStatusDetailShowRelativeTime;
			set => SetProperty(ref _timelineStatusDetailShowRelativeTime, value);
		}

		[JsonProperty("timeline_font_text_rendering")]
		private TextFormattingMode _timelineFontRendering = TextFormattingMode.Display;
		public TextFormattingMode TimelineFontRendering
		{
			get => _timelineFontRendering;
			set => SetProperty(ref _timelineFontRendering, value);
		}

		[JsonProperty("timeline_enable_item_animation")]
		private bool _enableTimelineAnimation = true;
		public bool EnableTimelineAnimation
		{
			get => _enableTimelineAnimation;
			set => SetProperty(ref _enableTimelineAnimation, value);
		}

		[JsonProperty("timeline_disable_animation_at_rdp")]
		private bool _disableAnimationAtTerminalConnection;
		public bool DisableAnimationAtTerminalConnection
		{
			get => _disableAnimationAtTerminalConnection;
			set => SetProperty(ref _disableAnimationAtTerminalConnection, value);
		}

		[JsonProperty("timeline_status_show_action_button")]
		private bool _timelineStatusActionButtonVisible = true;
		public bool TimelineStatusActionButtonVsiible
		{
			get => _timelineStatusActionButtonVisible;
			set => SetProperty(ref _timelineStatusActionButtonVisible, value);
		}

		[JsonProperty("timeline_status_detail_show_action_button")]
		private bool _timelineStatusDetailActionButtonVisible = true;
		public bool TimelineStatusDetailActionButtonVsiible
		{
			get => _timelineStatusDetailActionButtonVisible;
			set => SetProperty(ref _timelineStatusDetailActionButtonVisible, value);
		}


		#endregion

		#region NowPlaying


		[JsonProperty("now_playing_default_player")]
		private string _nowPlayingDefaultPlayer;
		public string NowPlayingDefaultPlayer
		{
			get => _nowPlayingDefaultPlayer ?? (_nowPlayingDefaultPlayer = DefaultNowPlayingPlayer);
			set => SetProperty(ref _nowPlayingDefaultPlayer, value);
		}

		[JsonProperty("now_playing_format")]
		private string _nowPlayingFormat;
		public string NowPlayingFormat
		{
			get => _nowPlayingFormat ?? (_nowPlayingFormat = DefaultNowPlayingFormat);
			set => SetProperty(ref _nowPlayingFormat, value ?? DefaultNowPlayingFormat);
		}


		[JsonProperty("now_playing_set_thumbnails")]
		private bool _insertThumbnailAtNowPlaying;
		public bool InsertThumbnailAtNowPlayying
		{
			get => _insertThumbnailAtNowPlaying;
			set => SetProperty(ref _insertThumbnailAtNowPlaying, value);
		}

		#endregion

		#region Notification

		private DictionaryEx<NotifyCode, bool> _ne => App.NotificationEvents;

		[JsonProperty("notification_enable")]
		private bool _enableNotification = true;
		public bool EnableNotification
		{
			get => _enableNotification;
			set => SetProperty(ref _enableNotification, value);
		}

		[JsonProperty("notification_sound_path")]
		private string _notificationSoundFile;
		public string NotificationSoundFile
		{
			get => _notificationSoundFile ?? (_notificationSoundFile = Defines.DefaultSoundFile);
			set => SetProperty(ref _notificationSoundFile, value);
		}

		[JsonProperty("notification_sound_enable")]
		private bool _enableSoundNotification;
		public bool EnableSoundNotification
		{
			get => _enableSoundNotification;
			set => SetProperty(ref _enableSoundNotification, value);
		}

		[JsonProperty("EnablePopupNotification")]
		private bool _enablePopupNotification;
		public bool EnablePopupNotification
		{
			get => _enablePopupNotification;
			set => SetProperty(ref _enablePopupNotification, value);
		}

		[JsonProperty("notification_reply")]
		public bool Notification_Reply
		{
			get => _ne.GetOrAdd(NotifyCode.Reply, true);
			set => _ne[NotifyCode.Reply] = value;
		}

		[JsonProperty("notification_favorite")]
		public bool Notification_Favorite
		{
			get => _ne.GetOrAdd(NotifyCode.Favorite, true);
			set => _ne[NotifyCode.Favorite] = value;
		}

		[JsonProperty("notification_quoted_tweet")]
		public bool Notification_QuotedTweet
		{
			get => _ne.GetOrAdd(NotifyCode.QuotedTweet, true);
			set => _ne[NotifyCode.QuotedTweet] = value;
		}

		[JsonProperty("notification_retweet")]
		public bool Notification_Retweet
		{
			get => _ne.GetOrAdd(NotifyCode.Retweet, true);
			set => _ne[NotifyCode.Retweet] = value;
		}

		[JsonProperty("notification_retweeted_retweet")]
		public bool Notification_RetweetedRetweet
		{
			get => _ne.GetOrAdd(NotifyCode.RetweetedRetweet, true);
			set => _ne[NotifyCode.RetweetedRetweet] = value;
		}

		[JsonProperty("notification_favorited_retweet")]
		public bool Notification_FavoritedRetweet
		{
			get => _ne.GetOrAdd(NotifyCode.FavoritedRetweet, true);
			set => _ne[NotifyCode.FavoritedRetweet] = value;
		}

		[JsonProperty("notification_list_member_added")]
		public bool Notification_ListMemberAdded
		{
			get => _ne.GetOrAdd(NotifyCode.ListMemberAdded, true);
			set => _ne[NotifyCode.ListMemberAdded] = value;
		}

		[JsonProperty("notification_follow")]
		public bool Notification_Follow
		{
			get => _ne.GetOrAdd(NotifyCode.Follow, true);
			set => _ne[NotifyCode.Follow] = value;
		}

		[JsonProperty("notification_dm_received")]
		public bool Notification_DirectMessageReceived
		{
			get => _ne.GetOrAdd(NotifyCode.DirectMessageCreated, true);
			set => _ne[NotifyCode.DirectMessageCreated] = value;
		}

		#endregion

		#region Mute

		[JsonProperty("mute")]
		private FluidCollection<Mute> _mute;
		public FluidCollection<Mute> Mute => _mute ?? (_mute = new FluidCollection<Mute>());

		#endregion

		#region Post

		[JsonProperty("post_close_window")]
		private bool _closeWindowAfterPostComplated;
		public bool CloseWindowAfterPostComplated
		{
			get => _closeWindowAfterPostComplated;
			set => SetProperty(ref _closeWindowAfterPostComplated, value);
		}

		#endregion

		#region Network

		[JsonProperty("network_system_proxy")]
		private bool _useSystemProxy;
		public bool UseSystemProxy
		{
			get => _useSystemProxy;
			set => SetProperty(ref _useSystemProxy, value);
		}

		#endregion
	}

	internal enum BackgroundType
	{
		[EnumMember(Value = "none")]
		None,

		[EnumMember(Value = "color")]
		Color,

		[EnumMember(Value = "picture")]
		Picture,
	}

	internal enum NotifyCode
	{
		[EnumMember(Value = "reply")]
		Reply,

		[EnumMember(Value = "retweet")]
		Retweet,

		[EnumMember(Value = "direct_message_created")]
		DirectMessageCreated,

		[EnumMember(Value = "direct_message_deleted")]
		DirectMessageDeleted,

		[EnumMember(Value = "block")]
		Block,

		[EnumMember(Value = "unblock")]
		Unblock,

		[EnumMember(Value = "favorite")]
		Favorite,

		[EnumMember(Value = "unfavorite")]
		Unfavorite,

		[EnumMember(Value = "follow")]
		Follow,

		[EnumMember(Value = "unfollow")]
		Unfollow,

		[EnumMember(Value = "list_created")]
		ListCreated,

		[EnumMember(Value = "list_destroyed")]
		ListDestroyed,

		[EnumMember(Value = "list_updated")]
		ListUpdated,

		[EnumMember(Value = "list_member_added")]
		ListMemberAdded,

		[EnumMember(Value = "list_member_removed")]
		ListMemberRemoved,

		[EnumMember(Value = "list_user_subscribed")]
		ListUserSubscribed,

		[EnumMember(Value = "list_user_unsubscribed")]
		ListUserUnsubscribed,

		[EnumMember(Value = "user_update")]
		UserUpdate,

		[EnumMember(Value = "mute")]
		Mute,

		[EnumMember(Value = "unmute")]
		Unmute,

		[EnumMember(Value = "favorited_retweet")]
		FavoritedRetweet,

		[EnumMember(Value = "retweeted_retweet")]
		RetweetedRetweet,

		[EnumMember(Value = "quoted_tweet")]
		QuotedTweet
	}
}
