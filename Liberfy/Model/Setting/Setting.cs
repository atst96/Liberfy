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

namespace Liberfy
{
	[JsonObject]
	internal partial class Setting : NotificationObject
	{
		internal void NormalizeSettings()
		{
			if(DefaultColumns == null)
			{
				DefaultColumns = new FluidCollection<ColumnSetting>
				{
					new ColumnSetting(ColumnType.Home, Account.Dummy),
					new ColumnSetting(ColumnType.Notification, Account.Dummy),
					new ColumnSetting(ColumnType.Messages, Account.Dummy),
				};
			}
		}

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

		[JsonProperty("startup_minimized")]
		public bool MinimizeStartup { get; set; } = true;

		[JsonProperty("tasktray_show")]
		public bool ShowInTaskTray { get; set; } = false;

		[JsonProperty("tasktray_show_at_minimized")]
		public bool ShowInTaskTrayAtMinimzied { get; set; } = true;

		[JsonProperty("tasktray_balloon_at_minimized")]
		public bool ShowBalloonAtMinized { get; set; } = false;

		[JsonProperty("minimize_click_close_button")]
		public bool MinimizeAtCloseButtonClick { get; set; } = false;

		private BackgroundType _backgroundType = BackgroundType.None;
		private AlignmentX _imageAlignmentX = AlignmentX.Left;
		private AlignmentY _imageAlignmentY = AlignmentY.Top;
		private Stretch _imageStretch = Stretch.UniformToFill;
		private double _imageOpacity = 1.0d;
		private string _imagePath = string.Empty;

		[JsonProperty("supress_shutdown")]
		public bool SupressShutdown { get; set; } = false;

		[JsonProperty("check_update")]
		public bool CheckUpdate
		{
			get { return _checkUpdate; }
			set { SetProperty(ref _checkUpdate, value); }
		}

		[JsonProperty("background_type")]
		public BackgroundType BackgroundType
		{
			get { return _backgroundType; }
			set { SetProperty(ref _backgroundType, value); }
		}

		[JsonProperty("background_alignment_x")]
		public AlignmentX ImageAlignmentX
		{
			get { return _imageAlignmentX; }
			set { SetProperty(ref _imageAlignmentX, value); }
		}

		[JsonProperty("background_alignment_y")]
		public AlignmentY ImageAlignmentY
		{
			get { return _imageAlignmentY; }
			set { SetProperty(ref _imageAlignmentY, value); }
		}

		[JsonProperty("background_image_stretch")]
		public Stretch BackgroundImageStretch
		{
			get { return _imageStretch; }
			set { SetProperty(ref _imageStretch, value); }
		}

		[JsonProperty("background_image_opacity")]
		public double BackgroundImageOpacity
		{
			get { return _imageOpacity; }
			set { SetProperty(ref _imageOpacity, value); }
		}

		[JsonProperty("background_image_path")]
		public string BackgroundImagePath
		{
			get { return _imagePath; }
			set { SetProperty(ref _imagePath, value); }
		}

		#endregion

		#region Account

		[JsonProperty("columns_default")]
		public FluidCollection<ColumnSetting> DefaultColumns { get; private set; }

		#endregion

		#region View settings

		public static readonly string[] DefaultTimelineFont = { "Meiryo", "Segoe UI Symbol" };
		public const double DefaultTimelineFontSize = 12;

		private string[] _timelineFont = DefaultTimelineFont;
		private double _timelineFontSize = DefaultTimelineFontSize;

		[JsonProperty("timeline_fonts")]
		public string[] TimelineFont
		{
			get { return _timelineFont; }
			set { SetProperty(ref _timelineFont, value); }
		}

		[JsonProperty("timeline_font_size")]
		public double TimelineFontSize
		{
			get { return _timelineFontSize; }
			set { SetProperty(ref _timelineFontSize, value); }
		}

		[JsonProperty("timeline_show_images")]
		public bool ShowImageInTimeline
		{
			get { return GetValue<bool>("ShowImageInTimeline"); }
			set
			{
				SetValue("ShowImageInTimeline", value);
				RaisePropertyChanged(nameof(ShowImageInTimeline));
			}
		}

		[JsonProperty("timeline_show_images_detail")]
		public bool ShowImageInDetail
		{
			get { return GetValue<bool>("ShowImageInDetail"); }
			set
			{
				SetValue("ShowImageInDetail", value);
				RaisePropertyChanged("ShowImageInDetail");
			}
		}

		[JsonProperty("timeline_show_relative_time")]
		public bool RelativeTime
		{
			get { return GetValue<bool>("RelativeTime"); }
			set { SetValue("RelativeTime", value); }
		}

		[JsonProperty("timeline_font_text_rendering")]
		public TextFormattingMode TextFormattingMode
		{
			get { return GetValue<TextFormattingMode>("TextFormattingMode"); }
			set
			{
				SetValue("TextFormattingMode", value);
				RaisePropertyChanged("TextFormattingMode");
			}
		}

		[JsonProperty("timeline_enable_item_animation")]
		public bool EnableTimelineAnimation { get; set; } = true;

		[JsonProperty("timeline_disable_animation_at_rdp")]
		public bool DisableAnimationAtTerminalConnection { get; set; } = false;

		[JsonProperty("timeline_items_show_action_button")]
		public bool ShowActionButtonInTimeline
		{
			get { return GetValue<bool>("ShowActionButtonInTimeline"); }
			set { SetValue("ShowActionButtonInTimeline", value); }
		}

		#endregion

		#region Format settings

		private string _nowPlayingFormat;

		[JsonProperty("format_now_playing")]
		public string NowPlayingFormat
		{
			get { return _nowPlayingFormat ?? (_nowPlayingFormat = Defines.DefaultNowPlayingFormat); }
			set { SetProperty(ref _nowPlayingFormat, value ?? Defines.DefaultNowPlayingFormat); }
		}


		private bool _insertThumbnailAtNowPlaying;

		[JsonProperty("now_playing_set_thumbnails")]
		public bool InsertThumbnailAtNowPlayying
		{
			get { return _insertThumbnailAtNowPlaying; }
			set { SetProperty(ref _insertThumbnailAtNowPlaying, value); }
		}

		#endregion

		#region Notification settings

		private DictionaryEx<NotifyCode, bool> _ne = App.NotificationEvents;

		private bool _enableNotification = true;
		[JsonProperty("notification_enable")]
		public bool EnableNotification
		{
			get { return _enableNotification; }
			set { SetProperty(ref _enableNotification, value); }
		}

		private string _notificationSoundFile;
		[JsonProperty("notification_sound_path")]
		public string NotificationSoundFile
		{
			get { return _notificationSoundFile ?? (_notificationSoundFile = Defines.DefaultSoundFile); }
			set { SetProperty(ref _notificationSoundFile, value); }
		}

		private bool _enableSoundNotification;
		[JsonProperty("notification_sound_enable")]
		public bool EnableSoundNotification
		{
			get { return _enableSoundNotification; }
			set { SetProperty(ref _enableSoundNotification, value); }
		}

		private bool _enablePopupNotification;
		[JsonProperty("EnablePopupNotification")]
		public bool EnablePopupNotification
		{
			get { return _enablePopupNotification; }
			set { SetProperty(ref _enablePopupNotification, value); }
		}

		[JsonProperty("notification_reply")]
		public bool Notification_Reply
		{
			get { return _ne.GetOrAdd(NotifyCode.Reply, true); }
			set { _ne[NotifyCode.Reply] = value; }
		}

		[JsonProperty("notification_favorite")]
		public bool Notification_Favorite
		{
			get { return _ne.GetOrAdd(NotifyCode.Favorite, true); }
			set { _ne[NotifyCode.Favorite] = value; }
		}

		[JsonProperty("notification_quoted_tweet")]
		public bool Notification_QuotedTweet
		{
			get { return _ne.GetOrAdd(NotifyCode.QuotedTweet, true); }
			set { _ne[NotifyCode.QuotedTweet] = value; }
		}

		[JsonProperty("notification_retweet")]
		public bool Notification_Retweet
		{
			get { return _ne.GetOrAdd(NotifyCode.Retweet, true); }
			set { _ne[NotifyCode.Retweet] = value; }
		}

		[JsonProperty("notification_retweeted_retweet")]
		public bool Notification_RetweetedRetweet
		{
			get { return _ne.GetOrAdd(NotifyCode.RetweetedRetweet, true); }
			set { _ne[NotifyCode.RetweetedRetweet] = value; }
		}

		[JsonProperty("notification_favorited_retweet")]
		public bool Notification_FavoritedRetweet
		{
			get { return _ne.GetOrAdd(NotifyCode.FavoritedRetweet, true); }
			set { _ne[NotifyCode.FavoritedRetweet] = value; }
		}

		[JsonProperty("notification_list_member_added")]
		public bool Notification_ListMemberAdded
		{
			get { return _ne.GetOrAdd(NotifyCode.ListMemberAdded, true); }
			set { _ne[NotifyCode.ListMemberAdded] = value; }
		}

		[JsonProperty("notification_follow")]
		public bool Notification_Follow
		{
			get { return _ne.GetOrAdd(NotifyCode.Follow, true); }
			set { _ne[NotifyCode.Follow] = value; }
		}

		[JsonProperty("notification_dm_received")]
		public bool Notification_DirectMessageReceived
		{
			get { return _ne.GetOrAdd(NotifyCode.DirectMessageCreated, true); }
			set { _ne[NotifyCode.DirectMessageCreated] = value; }
		}

		#endregion

		#region Mute settings

		private FluidCollection<Mute> _mute;
		[JsonProperty("mute")]
		public FluidCollection<Mute> Mute => _mute ?? (_mute = new FluidCollection<Mute>());

		#endregion

		#region Post settings

		[JsonProperty("post_close_window")]
		public bool CloseWindowAfterPostComplated { get; set; }

		#endregion

		#region Network settings

		[JsonProperty("network_system_proxy")]
		public bool UseSystemProxy { get; set; }

		#endregion
	}

	enum BackgroundType
	{
		[EnumMember(Value = "none")]
		None,

		[EnumMember(Value = "color")]
		Color,

		[EnumMember(Value = "picture")]
		Picture,
	}

	enum NotifyCode
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
