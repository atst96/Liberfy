using Liberfy.ViewModel;
using Liberfy.ViewModel.Column.Options;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using static Liberfy.Defines;

namespace Liberfy
{
    [DataContract]
    internal class Setting : NotificationObject
    {
        #region Generic

        [DataMember(Name = "startup_check_update")]
        private bool _checkUpdate = true;
        [IgnoreDataMember]
        public bool CheckUpdate
        {
            get => _checkUpdate;
            set => SetProperty(ref _checkUpdate, value);
        }

        [DataMember(Name = "startup_minimized")]
        private bool _minimizeStartup;
        [IgnoreDataMember]
        public bool MinimizeStartup
        {
            get => _minimizeStartup;
            set => SetProperty(ref _minimizeStartup, value);
        }

        [DataMember(Name = "tasktray_show")]
        private bool _showInTaskTray;
        [IgnoreDataMember]
        public bool ShowInTaskTray
        {
            get => _showInTaskTray;
            set => SetProperty(ref _showInTaskTray, value);
        }

        [DataMember(Name = "tasktray_show_at_minimized")]
        private bool _showInTaskTrayAtMinimized = true;
        [IgnoreDataMember]
        public bool ShowInTaskTrayAtMinimzied
        {
            get => _showInTaskTrayAtMinimized;
            set => SetProperty(ref _showInTaskTrayAtMinimized, value);
        }

        [DataMember(Name = "tasktray_balloon_at_minimized")]
        private bool _showBalloonAtMinimized = true;
        [IgnoreDataMember]
        public bool ShowBalloonAtMinimized
        {
            get => _showBalloonAtMinimized;
            set => SetProperty(ref _showBalloonAtMinimized, value);
        }

        [DataMember(Name = "minimize_click_close_button")]
        private bool _minimizeAtCloseButtonClick;
        [IgnoreDataMember]
        public bool MinimizeAtCloseButtonClick
        {
            get => _minimizeAtCloseButtonClick;
            set => SetProperty(ref _minimizeAtCloseButtonClick, value);
        }


        [DataMember(Name = "system_suppress_shutdown")]
        private bool _suppressShutdown;
        [IgnoreDataMember]
        public bool SuppressShutdown
        {
            get => _suppressShutdown;
            set => SetProperty(ref _suppressShutdown, value);
        }

        [DataMember(Name = "system_suppress_suspend")]
        private bool _suppressSuspend;
        [IgnoreDataMember]
        public bool SuppressSuspend
        {
            get => _suppressSuspend;
            set => SetProperty(ref _suppressSuspend, value);
        }

        [DataMember(Name = "system_suppress_screensaver")]
        private bool _suppressScreenSaver;
        [IgnoreDataMember]
        public bool SuppressScreenSaver
        {
            get => _suppressScreenSaver;
            set => SetProperty(ref _suppressScreenSaver, value);
        }

        [DataMember(Name = "background_type")]
        private BackgroundType _backgroundType = BackgroundType.None;
        [IgnoreDataMember]
        public BackgroundType BackgroundType
        {
            get => _backgroundType;
            set => SetProperty(ref _backgroundType, value);
        }

        [DataMember(Name = "background_alignment_x")]
        private AlignmentX _imageAlignmentX;
        [IgnoreDataMember]
        public AlignmentX ImageAlignmentX
        {
            get => _imageAlignmentX;
            set => SetProperty(ref _imageAlignmentX, value);
        }

        [DataMember(Name = "background_alignment_y")]
        private AlignmentY _imageAlignmentY = AlignmentY.Top;
        [IgnoreDataMember]
        public AlignmentY ImageAlignmentY
        {
            get => _imageAlignmentY;
            set => SetProperty(ref _imageAlignmentY, value);
        }

        [DataMember(Name = "background_image_stretch")]
        private Stretch _backgroundImageStretch = Stretch.UniformToFill;
        [IgnoreDataMember]
        public Stretch BackgroundImageStretch
        {
            get => _backgroundImageStretch;
            set => SetProperty(ref _backgroundImageStretch, value);
        }

        [DataMember(Name = "background_image_opacity")]
        private double _backgroundOpacity = 1.0d;
        [IgnoreDataMember]
        public double BackgroundImageOpacity
        {
            get => _backgroundOpacity;
            set => SetProperty(ref _backgroundOpacity, value);
        }

        [DataMember(Name = "background_image_path")]
        private string _backgroundImagePath;
        [IgnoreDataMember]
        public string BackgroundImagePath
        {
            get => _backgroundImagePath;
            set => SetProperty(ref _backgroundImagePath, value);
        }

        #endregion

        #region Account

        [DataMember(Name = "account_column_defaults")]
        [Utf8Json.JsonFormatter(typeof(FluidColumnOptionFormatter))]
        private FluidCollection<ColumnOptionBase> _defaultColumns;
        [IgnoreDataMember]
        public FluidCollection<ColumnOptionBase> DefaultColumns
        {
            get
            {
                if (_defaultColumns != null)
                {
                    return _defaultColumns;
                }
                else
                {
                    var defaultOptions = new[]
                    {
                        new GeneralColumnOption(ColumnType.Home),
                        new GeneralColumnOption(ColumnType.Notification),
                        new GeneralColumnOption(ColumnType.Messages)
                    };

                    return _defaultColumns = new FluidCollection<ColumnOptionBase>(defaultOptions);
                }
            }
        }

        [DataMember(Name = "account_default_automatically_login")]
        private bool _accountDefaultAutomaticallyLogin = true;
        [IgnoreDataMember]
        public bool AccountDefaultAutomaticallyLogin
        {
            get => _accountDefaultAutomaticallyLogin;
            set => SetProperty(ref _accountDefaultAutomaticallyLogin, value);
        }

        [DataMember(Name = "account_default_automatically_load_timeline")]
        private bool _accountDefaultAutomaticallyLoadTimeline = true;
        [IgnoreDataMember]
        public bool AccountDefaultAutomaticallyLoadTimeline
        {
            get => _accountDefaultAutomaticallyLoadTimeline;
            set => SetProperty(ref _accountDefaultAutomaticallyLoadTimeline, value);
        }

        #endregion

        #region View

        [DataMember(Name = "timeline_fonts")]
        private string[] _timelineFont;
        [IgnoreDataMember]
        public string[] TimelineFont
        {
            get => _timelineFont ?? (_timelineFont = DefaultTimelineFont);
            set => SetProperty(ref _timelineFont, value);
        }

        [DataMember(Name = "timeline_font_size")]
        private double _timelineFontSize;
        [IgnoreDataMember]
        public double TimelineFontSize
        {
            get => _timelineFontSize;
            set => SetProperty(ref _timelineFontSize, value);
        }


        [DataMember(Name = "timeline_tweet_show_media")]
        private bool _timelineStatusShowMedia = true;
        [IgnoreDataMember]
        public bool TimelineStatusShowMedia
        {
            get => _timelineStatusShowMedia;
            set => SetProperty(ref _timelineStatusShowMedia, value);
        }

        [DataMember(Name = "timeline_tweet_show_media_detail")]
        private bool _timelineStatusDetailShowMedia = true;
        [IgnoreDataMember]
        public bool TimelineStatusDetailShowMedia
        {
            get => _timelineStatusDetailShowMedia;
            set => SetProperty(ref _timelineStatusDetailShowMedia, value);
        }

        [DataMember(Name = "timeline_tweet_show_quoted_tweet")]
        private bool _timelineStatusShowQuotedTweet = true;
        [IgnoreDataMember]
        public bool TimelineStatusShowQuotedTweet
        {
            get => _timelineStatusDetailShowMedia;
            set => SetProperty(ref _timelineStatusShowQuotedTweet, value);
        }

        [DataMember(Name = "timeline_tweet_show_quoted_tweet_detail")]
        private bool _timelineStatusDetailShowQuotedTweet = true;
        [IgnoreDataMember]
        public bool TimelineStatusDetailShowQuotedTweet
        {
            get => _timelineStatusDetailShowQuotedTweet;
            set => SetProperty(ref _timelineStatusDetailShowQuotedTweet, value);
        }

        [DataMember(Name = "timeline_tweet_show_relative_time")]
        private bool _timelineStatusShowRelativeTime = true;
        [IgnoreDataMember]
        public bool TimelineStatusShowRelativeTime
        {
            get => _timelineStatusShowRelativeTime;
            set => SetProperty(ref _timelineStatusShowRelativeTime, value);
        }

        [DataMember(Name = "timeline_tweet_show_relatvie_time_detail")]
        private bool _timelineStatusDetailShowRelativeTime;
        [IgnoreDataMember]
        public bool TimelineStatusDetailShowRelativeTime
        {
            get => _timelineStatusDetailShowRelativeTime;
            set => SetProperty(ref _timelineStatusDetailShowRelativeTime, value);
        }

        [DataMember(Name = "timeline_font_text_rendering")]
        private TextFormattingMode _timelineFontRendering = TextFormattingMode.Display;
        [IgnoreDataMember]
        public TextFormattingMode TimelineFontRendering
        {
            get => _timelineFontRendering;
            set => SetProperty(ref _timelineFontRendering, value);
        }

        [DataMember(Name = "timeline_enable_item_animation")]
        private bool _enableTimelineAnimation = true;
        [IgnoreDataMember]
        public bool EnableTimelineAnimation
        {
            get => _enableTimelineAnimation;
            set => SetProperty(ref _enableTimelineAnimation, value);
        }

        [DataMember(Name = "timeline_disable_animation_at_rdp")]
        private bool _disableAnimationAtTerminalConnection;
        [IgnoreDataMember]
        public bool DisableAnimationAtTerminalConnection
        {
            get => _disableAnimationAtTerminalConnection;
            set => SetProperty(ref _disableAnimationAtTerminalConnection, value);
        }

        [DataMember(Name = "timeline_status_show_action_button")]
        private bool _timelineStatusActionButtonVisible = true;
        [IgnoreDataMember]
        public bool TimelineStatusActionButtonVsiible
        {
            get => _timelineStatusActionButtonVisible;
            set => SetProperty(ref _timelineStatusActionButtonVisible, value);
        }

        [DataMember(Name = "timeline_status_detail_show_action_button")]
        private bool _timelineStatusDetailActionButtonVisible = true;
        [IgnoreDataMember]
        public bool TimelineStatusDetailActionButtonVsiible
        {
            get => _timelineStatusDetailActionButtonVisible;
            set => SetProperty(ref _timelineStatusDetailActionButtonVisible, value);
        }

        [DataMember(Name = "tweet_profile_image_width")]
        private double _tweetProfileImageWidth = DefaultProfileImageWidth;
        [IgnoreDataMember]
        public double TweetProfileImageWidth
        {
            get => _tweetProfileImageWidth;
            set
            {
                double width = Math.Max(MinimumProfileImageWidth, Math.Min(Math.Floor(value), MaximumProfileImageWidth));
                SetProperty(ref _tweetProfileImageWidth, width);
            }
        }

        [DataMember(Name = "column_width")]
        private double _columnWidth = DefaultColumnWidth;
        [IgnoreDataMember]
        public double ColumnWidth
        {
            get => _columnWidth;
            set
            {
                double width = Math.Max(MinimumColumnWidth, Math.Min(Math.Floor(value), MaximumColumnWidth));
                SetProperty(ref _columnWidth, width);
            }
        }

        [DataMember(Name = "tweet_show_profile_image")]
        private bool _isShowTweetProfileImage = true;
        [IgnoreDataMember]
        public bool IsShowTweetProfileImage
        {
            get => _isShowTweetProfileImage;
            set => SetProperty(ref _isShowTweetProfileImage, value);
        }

        [DataMember(Name = "tweet_show_images")]
        private bool _isShowTweetImages = true;
        [IgnoreDataMember]
        public bool IsShowTweetImages
        {
            get => _isShowTweetImages;
            set => SetProperty(ref _isShowTweetImages, value);
        }

        [DataMember(Name = "tweet_show_quoted_tweet")]
        private bool _isShowTweetQuotedTweet = true;
        [IgnoreDataMember]
        public bool IsShowTweetQuotedTweet
        {
            get => _isShowTweetQuotedTweet;
            set => SetProperty(ref _isShowTweetQuotedTweet, value);
        }

        [DataMember(Name = "tweet_show_client_name")]
        private bool _isShowTweetClientName = true;
        [IgnoreDataMember]
        public bool IsShowTweetClientName
        {
            get => _isShowTweetClientName;
            set => SetProperty(ref _isShowTweetClientName, value);
        }

        [DataMember(Name = "tweet_profile_image_form")]
        private ProfileImageForm _profileImageForm = ProfileImageForm.Square;
        [IgnoreDataMember]
        public ProfileImageForm ProfileImageForm
        {
            get => _profileImageForm;
            set => SetProperty(ref _profileImageForm, value);
        }

        #endregion

        #region NowPlaying


        [DataMember(Name = "now_playing_default_player")]
        private string _nowPlayingDefaultPlayer;
        [IgnoreDataMember]
        public string NowPlayingDefaultPlayer
        {
            get => _nowPlayingDefaultPlayer ?? (_nowPlayingDefaultPlayer = DefaultNowPlayingPlayer);
            set => SetProperty(ref _nowPlayingDefaultPlayer, value);
        }

        [DataMember(Name = "now_playing_format")]
        private string _nowPlayingFormat;
        [IgnoreDataMember]
        public string NowPlayingFormat
        {
            get => _nowPlayingFormat ?? (_nowPlayingFormat = DefaultNowPlayingFormat);
            set => SetProperty(ref _nowPlayingFormat, value ?? DefaultNowPlayingFormat);
        }


        [DataMember(Name = "now_playing_set_thumbnails")]
        private bool _insertThumbnailAtNowPlaying;
        [IgnoreDataMember]
        public bool InsertThumbnailAtNowPlayying
        {
            get => _insertThumbnailAtNowPlaying;
            set => SetProperty(ref _insertThumbnailAtNowPlaying, value);
        }

        #endregion

        #region Notification

        [IgnoreDataMember]
        private DictionaryEx<NotifyCode, bool> _ne => App.NotificationEvents;

        [DataMember(Name = "notification_enable")]
        private bool _enableNotification = true;
        [IgnoreDataMember]
        public bool EnableNotification
        {
            get => _enableNotification;
            set => SetProperty(ref _enableNotification, value);
        }

        [DataMember(Name = "notification_sound_path")]
        private string _notificationSoundFile;
        [IgnoreDataMember]
        public string NotificationSoundFile
        {
            get => _notificationSoundFile ?? (_notificationSoundFile = Defines.DefaultSoundFile);
            set => SetProperty(ref _notificationSoundFile, value);
        }

        [DataMember(Name = "notification_sound_enable")]
        private bool _enableSoundNotification;
        [IgnoreDataMember]
        public bool EnableSoundNotification
        {
            get => _enableSoundNotification;
            set => SetProperty(ref _enableSoundNotification, value);
        }

        [DataMember(Name = "notification_popup_enabled")]
        private bool _enablePopupNotification;
        [IgnoreDataMember]
        public bool EnablePopupNotification
        {
            get => _enablePopupNotification;
            set => SetProperty(ref _enablePopupNotification, value);
        }

        [DataMember(Name = "notification_reply")]
        public bool Notification_Reply
        {
            get => _ne.GetOrAdd(NotifyCode.Reply, true);
            set => _ne[NotifyCode.Reply] = value;
        }

        [DataMember(Name = "notification_favorite")]
        public bool Notification_Favorite
        {
            get => _ne.GetOrAdd(NotifyCode.Favorite, true);
            set => _ne[NotifyCode.Favorite] = value;
        }

        [DataMember(Name = "notification_quoted_tweet")]
        public bool Notification_QuotedTweet
        {
            get => _ne.GetOrAdd(NotifyCode.QuotedTweet, true);
            set => _ne[NotifyCode.QuotedTweet] = value;
        }

        [DataMember(Name = "notification_retweet")]
        public bool Notification_Retweet
        {
            get => _ne.GetOrAdd(NotifyCode.Retweet, true);
            set => _ne[NotifyCode.Retweet] = value;
        }

        [DataMember(Name = "notification_retweeted_retweet")]
        public bool Notification_RetweetedRetweet
        {
            get => _ne.GetOrAdd(NotifyCode.RetweetedRetweet, true);
            set => _ne[NotifyCode.RetweetedRetweet] = value;
        }

        [DataMember(Name = "notification_favorited_retweet")]
        public bool Notification_FavoritedRetweet
        {
            get => _ne.GetOrAdd(NotifyCode.FavoritedRetweet, true);
            set => _ne[NotifyCode.FavoritedRetweet] = value;
        }

        [DataMember(Name = "notification_list_member_added")]
        public bool Notification_ListMemberAdded
        {
            get => _ne.GetOrAdd(NotifyCode.ListMemberAdded, true);
            set => _ne[NotifyCode.ListMemberAdded] = value;
        }

        [DataMember(Name = "notification_follow")]
        public bool Notification_Follow
        {
            get => _ne.GetOrAdd(NotifyCode.Follow, true);
            set => _ne[NotifyCode.Follow] = value;
        }

        [DataMember(Name = "notification_dm_received")]
        public bool Notification_DirectMessageReceived
        {
            get => _ne.GetOrAdd(NotifyCode.DirectMessageCreated, true);
            set => _ne[NotifyCode.DirectMessageCreated] = value;
        }

        #endregion

        #region Mute

        [DataMember(Name = "mute")]
        private FluidCollection<Mute> _mute;
        [IgnoreDataMember]
        public FluidCollection<Mute> Mute => _mute ?? (_mute = new FluidCollection<Mute>());

        #endregion

        #region Post

        [DataMember(Name = "post_close_window")]
        private bool _closeWindowAfterPostComplated;
        [IgnoreDataMember]
        public bool CloseWindowAfterPostComplated
        {
            get => _closeWindowAfterPostComplated;
            set => SetProperty(ref _closeWindowAfterPostComplated, value);
        }

        [DataMember(Name = "post_reply_include_others")]
        private bool _includeOthersAtReply;
        [IgnoreDataMember]
        public bool IncludeOtherAtReply
        {
            get => _includeOthersAtReply;
            set => SetProperty(ref _includeOthersAtReply, value);
        }

        [DataMember(Name = "post_default_notice_sensitive_media")]
        private bool _noticePostSensitiveMedia;
        [IgnoreDataMember]
        public bool NoticePostSensitiveMedia
        {
            get => _noticePostSensitiveMedia;
            set => SetProperty(ref _noticePostSensitiveMedia, value);
        }

        #endregion

        #region Network

        [DataMember(Name = "network_system_proxy")]
        private bool _useSystemProxy;
        [IgnoreDataMember]
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

    public enum ProfileImageForm
    {
        Square,
        RoundedCorner,
        Ellipse,
    }
}
