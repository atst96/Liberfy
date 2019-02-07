using Liberfy.Settings;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using static Liberfy.Defines;

using Mastodon = SocialApis.Mastodon;

namespace Liberfy
{
    [DataContract]
    internal class Setting : NotificationObject
    {
        #region Generic

        [Key("general.check_update")]
        [DataMember(Name = "startup_check_update")]
        private bool _checkUpdate = true;
        [IgnoreDataMember]
        public bool CheckUpdate
        {
            get => _checkUpdate;
            set => SetProperty(ref _checkUpdate, value);
        }

        [Key("window.startup.is_minimized")]
        [DataMember(Name = "startup_minimized")]
        private bool _minimizeStartup;
        [IgnoreDataMember]
        public bool MinimizeStartup
        {
            get => _minimizeStartup;
            set => SetProperty(ref _minimizeStartup, value);
        }

        [Key("tasktray.show")]
        [DataMember(Name = "tasktray_show")]
        private bool _showInTaskTray;
        [IgnoreDataMember]
        public bool ShowInTaskTray
        {
            get => _showInTaskTray;
            set => SetProperty(ref _showInTaskTray, value);
        }

        [Key("tasktray.minimized")]
        [DataMember(Name = "tasktray_show_at_minimized")]
        private bool _showInTaskTrayAtMinimized = true;
        [IgnoreDataMember]
        public bool ShowInTaskTrayAtMinimzied
        {
            get => _showInTaskTrayAtMinimized;
            set => SetProperty(ref _showInTaskTrayAtMinimized, value);
        }

        [Key("window.main.no_close")]
        [DataMember(Name = "minimize_click_close_button")]
        private bool _minimizeAtCloseButtonClick;
        [IgnoreDataMember]
        public bool MinimizeAtCloseButtonClick
        {
            get => _minimizeAtCloseButtonClick;
            set => SetProperty(ref _minimizeAtCloseButtonClick, value);
        }

        [Key("system.signout.cancel")]
        [DataMember(Name = "system_signout_cancel")]
        private bool _systemCancelSignout;
        [IgnoreDataMember]
        public bool SystemCancelSignout
        {
            get => this._systemCancelSignout;
            set => this.SetProperty(ref this._systemCancelSignout, value);
        }

        [Key("system.shutdown.cancel")]
        [DataMember(Name = "system_shutdown_cancel")]
        private bool _systemCacnelShutdown;
        [IgnoreDataMember]
        public bool SystemCancelShutdown
        {
            get => this._systemCacnelShutdown;
            set => this.SetProperty(ref this._systemCacnelShutdown, value);
        }

        [Key("window.background.type")]
        [DataMember(Name = "background_type")]
        private BackgroundType _backgroundType = BackgroundType.None;
        [IgnoreDataMember]
        public BackgroundType BackgroundType
        {
            get => _backgroundType;
            set => SetProperty(ref _backgroundType, value);
        }

        [Key("window.main.background.image.horizontal")]
        [DataMember(Name = "background_alignment_x")]
        private AlignmentX _imageAlignmentX;
        [IgnoreDataMember]
        public AlignmentX ImageAlignmentX
        {
            get => _imageAlignmentX;
            set => SetProperty(ref _imageAlignmentX, value);
        }

        [Key("window.main.background.image.vertical")]
        [DataMember(Name = "background_alignment_y")]
        private AlignmentY _imageAlignmentY = AlignmentY.Top;
        [IgnoreDataMember]
        public AlignmentY ImageAlignmentY
        {
            get => _imageAlignmentY;
            set => SetProperty(ref _imageAlignmentY, value);
        }

        [Key("window.main.background.image.stretch")]
        [DataMember(Name = "background_image_stretch")]
        private Stretch _backgroundImageStretch = Stretch.UniformToFill;
        [IgnoreDataMember]
        public Stretch BackgroundImageStretch
        {
            get => _backgroundImageStretch;
            set => SetProperty(ref _backgroundImageStretch, value);
        }

        [Key("window.main.background.image.opacity")]
        [DataMember(Name = "background_image_opacity")]
        private double _backgroundOpacity = 1.0d;
        [IgnoreDataMember]
        public double BackgroundImageOpacity
        {
            get => _backgroundOpacity;
            set => SetProperty(ref _backgroundOpacity, value);
        }

        [Key("window.main.background.image.path")]
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

        [Key("accounts.column.defaults")]
        [DataMember(Name = "account_column_defaults")]
        private NotifiableCollection<ColumnSetting> _defaultColumns;
        [IgnoreDataMember]
        public NotifiableCollection<ColumnSetting> DefaultColumns
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
                        new ColumnSetting{ Type = ColumnType.Home },
                        new ColumnSetting{ Type = ColumnType.Notification },
                        new ColumnSetting{ Type = ColumnType.Messages },
                    };

                    return _defaultColumns = new NotifiableCollection<ColumnSetting>(defaultOptions);
                }
            }
        }

        [Key("account.load.get_muted")]
        [DataMember(Name = "account_loading_get_muted")]
        private bool _getMutedIdsAtLoadingAccount = true;
        [IgnoreDataMember]
        public bool GetMutedIdsAtLoadingAccount
        {
            get => this._getMutedIdsAtLoadingAccount;
            set => this.SetProperty(ref this._getMutedIdsAtLoadingAccount, value);
        }

        [Key("account.load.get_blocked")]
        [DataMember(Name = "account_loading_get_blocked")]
        private bool _getBlockedIdsAtLoadingAccount = true;
        [IgnoreDataMember]
        public bool GetBlockedIdsAtLoadingAccount
        {
            get => this._getBlockedIdsAtLoadingAccount;
            set => this.SetProperty(ref this._getBlockedIdsAtLoadingAccount, value);
        }

        #endregion

        #region View

        [Key("timeline.font.families")]
        [DataMember(Name = "timeline_fonts")]
        private string[] _timelineFont;
        [IgnoreDataMember]
        public string[] TimelineFont
        {
            get => _timelineFont ?? (_timelineFont = DefaultTimelineFont);
            set => SetProperty(ref _timelineFont, value);
        }

        [Key("timeline.font.size")]
        [DataMember(Name = "timeline_font_size")]
        private double _timelineFontSize;
        [IgnoreDataMember]
        public double TimelineFontSize
        {
            get => _timelineFontSize;
            set => SetProperty(ref _timelineFontSize, value);
        }

        [Key("timeline.list.show_media")]
        [DataMember(Name = "timeline_tweet_show_media")]
        private bool _timelineStatusShowMedia = true;
        [IgnoreDataMember]
        public bool TimelineStatusShowMedia
        {
            get => _timelineStatusShowMedia;
            set => SetProperty(ref _timelineStatusShowMedia, value);
        }

        [Key("timeline.single.show_media")]
        [DataMember(Name = "timeline_tweet_show_media_detail")]
        private bool _timelineStatusDetailShowMedia = true;
        [IgnoreDataMember]
        public bool TimelineStatusDetailShowMedia
        {
            get => _timelineStatusDetailShowMedia;
            set => SetProperty(ref _timelineStatusDetailShowMedia, value);
        }

        [Key("timeline.list.show_quoted")]
        [DataMember(Name = "timeline_tweet_show_quoted_tweet")]
        private bool _timelineStatusShowQuotedTweet = true;
        [IgnoreDataMember]
        public bool TimelineStatusShowQuotedTweet
        {
            get => _timelineStatusDetailShowMedia;
            set => SetProperty(ref _timelineStatusShowQuotedTweet, value);
        }

        [Key("timeline.single.show_quoted")]
        [DataMember(Name = "timeline_tweet_show_quoted_tweet_detail")]
        private bool _timelineStatusDetailShowQuotedTweet = true;
        [IgnoreDataMember]
        public bool TimelineStatusDetailShowQuotedTweet
        {
            get => _timelineStatusDetailShowQuotedTweet;
            set => SetProperty(ref _timelineStatusDetailShowQuotedTweet, value);
        }

        [Key("timeline.list.is_time_relative")]
        [DataMember(Name = "timeline_tweet_show_relative_time")]
        private bool _timelineStatusShowRelativeTime = true;
        [IgnoreDataMember]
        public bool TimelineStatusShowRelativeTime
        {
            get => _timelineStatusShowRelativeTime;
            set => SetProperty(ref _timelineStatusShowRelativeTime, value);
        }

        [Key("timeline.single.is_time_relative")]
        [DataMember(Name = "timeline_tweet_show_relatvie_time_detail")]
        private bool _timelineStatusDetailShowRelativeTime;
        [IgnoreDataMember]
        public bool TimelineStatusDetailShowRelativeTime
        {
            get => _timelineStatusDetailShowRelativeTime;
            set => SetProperty(ref _timelineStatusDetailShowRelativeTime, value);
        }

        [Key("timeline.font.rendering")]
        [DataMember(Name = "timeline_font_text_rendering")]
        private TextFormattingMode _timelineFontRendering = TextFormattingMode.Display;
        [IgnoreDataMember]
        public TextFormattingMode TimelineFontRendering
        {
            get => _timelineFontRendering;
            set => SetProperty(ref _timelineFontRendering, value);
        }

        [Key("timeline.list.buttons.show")]
        [DataMember(Name = "timeline_status_show_action_button")]
        private bool _timelineStatusActionButtonVisible = true;
        [IgnoreDataMember]
        public bool TimelineStatusActionButtonVsiible
        {
            get => _timelineStatusActionButtonVisible;
            set => SetProperty(ref _timelineStatusActionButtonVisible, value);
        }

        [Key("timeline.single.buttons.show")]
        [DataMember(Name = "timeline_status_detail_show_action_button")]
        private bool _timelineStatusDetailActionButtonVisible = true;
        [IgnoreDataMember]
        public bool TimelineStatusDetailActionButtonVsiible
        {
            get => _timelineStatusDetailActionButtonVisible;
            set => SetProperty(ref _timelineStatusDetailActionButtonVisible, value);
        }

        [Key("timeline.profile_image.width")]
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

        [Key("timeline.columns.width")]
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

        [Key("timeline.profile_image.show")]
        [DataMember(Name = "tweet_show_profile_image")]
        private bool _isShowTweetProfileImage = true;
        [IgnoreDataMember]
        public bool IsShowTweetProfileImage
        {
            get => _isShowTweetProfileImage;
            set => SetProperty(ref _isShowTweetProfileImage, value);
        }

        [Key("timeline.attachment.images.show")]
        [DataMember(Name = "tweet_show_images")]
        private bool _isShowTweetImages = true;
        [IgnoreDataMember]
        public bool IsShowTweetImages
        {
            get => _isShowTweetImages;
            set => SetProperty(ref _isShowTweetImages, value);
        }

        [Key("timeline.attachment.quoted_status.show")]
        [DataMember(Name = "tweet_show_quoted_tweet")]
        private bool _isShowTweetQuotedTweet = true;
        [IgnoreDataMember]
        public bool IsShowTweetQuotedTweet
        {
            get => _isShowTweetQuotedTweet;
            set => SetProperty(ref _isShowTweetQuotedTweet, value);
        }

        [Key("timeline.meta.client_name.show")]
        [DataMember(Name = "tweet_show_client_name")]
        private bool _isShowTweetClientName = true;
        [IgnoreDataMember]
        public bool IsShowTweetClientName
        {
            get => _isShowTweetClientName;
            set => SetProperty(ref _isShowTweetClientName, value);
        }

        [Key("timeline.profile_image.form")]
        [DataMember(Name = "tweet_profile_image_form")]
        private ProfileImageForm _profileImageForm = ProfileImageForm.Square;
        [IgnoreDataMember]
        public ProfileImageForm ProfileImageForm
        {
            get => _profileImageForm;
            set => SetProperty(ref _profileImageForm, value);
        }

        #endregion

        #region Apis

        [Key("services.mastodon.keys")]
        [DataMember(Name = "mastodon_apis")]
        private NotifiableCollection<ClientKeyCache> _clientKeys;
        [IgnoreDataMember]
        public NotifiableCollection<ClientKeyCache> ClientKeys
        {
            get => this._clientKeys ?? (this._clientKeys = new NotifiableCollection<ClientKeyCache>());
        }

        #endregion

        #region NowPlaying

        [Key("post.nowplaying.format")]
        [DataMember(Name = "now_playing_format")]
        private string _nowPlayingFormat;
        [IgnoreDataMember]
        public string NowPlayingFormat
        {
            get => _nowPlayingFormat ?? (_nowPlayingFormat = DefaultNowPlayingFormat);
            set => SetProperty(ref _nowPlayingFormat, value ?? DefaultNowPlayingFormat);
        }

        [Key("post.nowplaying.insert_thumbnails")]
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

        [Key("notification.enabled")]
        [DataMember(Name = "notification_enable")]
        private bool _enableNotification = true;
        [IgnoreDataMember]
        public bool EnableNotification
        {
            get => _enableNotification;
            set => SetProperty(ref _enableNotification, value);
        }

        [Key("notification.sound.path")]
        [DataMember(Name = "notification_sound_path")]
        private string _notificationSoundFile;
        [IgnoreDataMember]
        public string NotificationSoundFile
        {
            get => _notificationSoundFile ?? (_notificationSoundFile = Defines.DefaultSoundFile);
            set => SetProperty(ref _notificationSoundFile, value);
        }

        [Key("notification.sound.enabled")]
        [DataMember(Name = "notification_sound_enable")]
        private bool _enableSoundNotification;
        [IgnoreDataMember]
        public bool EnableSoundNotification
        {
            get => _enableSoundNotification;
            set => SetProperty(ref _enableSoundNotification, value);
        }

        [Key("notification.balloon.enabled")]
        [DataMember(Name = "notification_popup_enabled")]
        private bool _enablePopupNotification;
        [IgnoreDataMember]
        public bool EnablePopupNotification
        {
            get => _enablePopupNotification;
            set => SetProperty(ref _enablePopupNotification, value);
        }

        [Key("notification.reply")]
        [DataMember(Name = "notification_reply")]
        public bool Notification_Reply
        {
            get => _ne.GetOrAdd(NotifyCode.Reply, true);
            set => _ne[NotifyCode.Reply] = value;
        }

        [Key("notifiation.favorite")]
        [DataMember(Name = "notification_favorite")]
        public bool Notification_Favorite
        {
            get => _ne.GetOrAdd(NotifyCode.Favorite, true);
            set => _ne[NotifyCode.Favorite] = value;
        }

        [Key("notification.quoted_tweet")]
        [DataMember(Name = "notification_quoted_tweet")]
        public bool Notification_QuotedTweet
        {
            get => _ne.GetOrAdd(NotifyCode.QuotedTweet, true);
            set => _ne[NotifyCode.QuotedTweet] = value;
        }

        [Key("notification.retweet")]
        [DataMember(Name = "notification_retweet")]
        public bool Notification_Retweet
        {
            get => _ne.GetOrAdd(NotifyCode.Retweet, true);
            set => _ne[NotifyCode.Retweet] = value;
        }

        [Key("notification.retweeted_retweet")]
        [DataMember(Name = "notification_retweeted_retweet")]
        public bool Notification_RetweetedRetweet
        {
            get => _ne.GetOrAdd(NotifyCode.RetweetedRetweet, true);
            set => _ne[NotifyCode.RetweetedRetweet] = value;
        }

        [Key("notification.favorited_retweet")]
        [DataMember(Name = "notification_favorited_retweet")]
        public bool Notification_FavoritedRetweet
        {
            get => _ne.GetOrAdd(NotifyCode.FavoritedRetweet, true);
            set => _ne[NotifyCode.FavoritedRetweet] = value;
        }

        [Key("notification.list_member.added")]
        [DataMember(Name = "notification_list_member_added")]
        public bool Notification_ListMemberAdded
        {
            get => _ne.GetOrAdd(NotifyCode.ListMemberAdded, true);
            set => _ne[NotifyCode.ListMemberAdded] = value;
        }

        [Key("notification.follow")]
        [DataMember(Name = "notification_follow")]
        public bool Notification_Follow
        {
            get => _ne.GetOrAdd(NotifyCode.Follow, true);
            set => _ne[NotifyCode.Follow] = value;
        }

        [Key("notification.dm_received")]
        [DataMember(Name = "notification_dm_received")]
        public bool Notification_DirectMessageReceived
        {
            get => _ne.GetOrAdd(NotifyCode.DirectMessageCreated, true);
            set => _ne[NotifyCode.DirectMessageCreated] = value;
        }

        #endregion

        #region Mute

        [Key("mute.list")]
        [DataMember(Name = "mute")]
        private NotifiableCollection<Mute> _mute;
        [IgnoreDataMember]
        public NotifiableCollection<Mute> Mute => _mute ?? (_mute = new NotifiableCollection<Mute>());

        #endregion

        #region Post

        [Key("post.close_window")]
        [DataMember(Name = "post_close_window")]
        private bool _closeWindowAfterPostComplated;
        [IgnoreDataMember]
        public bool CloseWindowAfterPostComplated
        {
            get => _closeWindowAfterPostComplated;
            set => SetProperty(ref _closeWindowAfterPostComplated, value);
        }

        [Key("post.reply.include_accounts")]
        [DataMember(Name = "post_reply_include_others")]
        private bool _includeOthersAtReply;
        [IgnoreDataMember]
        public bool IncludeOtherAtReply
        {
            get => _includeOthersAtReply;
            set => SetProperty(ref _includeOthersAtReply, value);
        }

        [Key("post.notice_sensitive")]
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

        [Key("network.use_system_proxy")]
        [DataMember(Name = "network_system_proxy")]
        private bool _useSystemProxy;
        [IgnoreDataMember]
        public bool UseSystemProxy
        {
            get => _useSystemProxy;
            set => SetProperty(ref _useSystemProxy, value);
        }

        #endregion

        #region Window

        [Key("window")]
        [DataMember(Name = "window")]
        private WindowSettings _window;
        [IgnoreDataMember]
        public WindowSettings Window => this._window ?? (this._window = new WindowSettings());

        #endregion Window
    }

    internal enum BackgroundType : ushort
    {
        [EnumMember(Value = "none")]
        None = 0,

        [EnumMember(Value = "color")]
        Color = 1,

        [EnumMember(Value = "picture")]
        Picture = 2,
    }

    internal enum NotifyCode : ushort
    {
        [EnumMember(Value = "reply")]
        Reply = 0,

        [EnumMember(Value = "retweet")]
        Retweet = 1,

        [EnumMember(Value = "direct_message_created")]
        DirectMessageCreated = 2,

        [EnumMember(Value = "direct_message_deleted")]
        DirectMessageDeleted = 3,

        [EnumMember(Value = "block")]
        Block = 4,

        [EnumMember(Value = "unblock")]
        Unblock = 5,

        [EnumMember(Value = "favorite")]
        Favorite = 6,

        [EnumMember(Value = "unfavorite")]
        Unfavorite = 7,

        [EnumMember(Value = "follow")]
        Follow = 8,

        [EnumMember(Value = "unfollow")]
        Unfollow = 9,

        [EnumMember(Value = "list_created")]
        ListCreated = 10,

        [EnumMember(Value = "list_destroyed")]
        ListDestroyed = 11,

        [EnumMember(Value = "list_updated")]
        ListUpdated = 12,

        [EnumMember(Value = "list_member_added")]
        ListMemberAdded = 13,

        [EnumMember(Value = "list_member_removed")]
        ListMemberRemoved = 14,

        [EnumMember(Value = "list_user_subscribed")]
        ListUserSubscribed = 15,

        [EnumMember(Value = "list_user_unsubscribed")]
        ListUserUnsubscribed = 16,

        [EnumMember(Value = "user_update")]
        UserUpdate = 17,

        [EnumMember(Value = "mute")]
        Mute = 18,

        [EnumMember(Value = "unmute")]
        Unmute = 19,

        [EnumMember(Value = "favorited_retweet")]
        FavoritedRetweet = 20,

        [EnumMember(Value = "retweeted_retweet")]
        RetweetedRetweet = 21,

        [EnumMember(Value = "quoted_tweet")]
        QuotedTweet = 22
    }

    public enum ProfileImageForm : byte
    {
        Square = 0,
        RoundedCorner = 1,
        Ellipse = 2,
    }
}
