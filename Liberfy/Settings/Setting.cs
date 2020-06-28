using Liberfy.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using static Liberfy.Defaults;

using Mastodon = SocialApis.Mastodon;

namespace Liberfy
{
    [DataContract]
    internal class Setting : NotificationObject, IJsonFile
    {
        #region Generic

        void IJsonFile.OnSerialize()
        {
        }

        void IJsonFile.OnDeserialized()
        {
        }

        [DataMember(Name = "startup_check_update")]
        private bool _checkUpdate = true;
        [IgnoreDataMember]
        public bool CheckUpdate
        {
            get => this._checkUpdate;
            set => this.SetProperty(ref this._checkUpdate, value);
        }

        [DataMember(Name = "startup_minimized")]
        private bool _minimizeStartup;
        [IgnoreDataMember]
        public bool MinimizeStartup
        {
            get => this._minimizeStartup;
            set => this.SetProperty(ref this._minimizeStartup, value);
        }

        [DataMember(Name = "tasktray_show")]
        private bool _showInTaskTray;
        [IgnoreDataMember]
        public bool ShowInTaskTray
        {
            get => this._showInTaskTray;
            set => this.SetProperty(ref this._showInTaskTray, value);
        }

        [DataMember(Name = "tasktray_show_at_minimized")]
        private bool _showInTaskTrayAtMinimized = true;
        [IgnoreDataMember]
        public bool ShowInTaskTrayAtMinimzied
        {
            get => this._showInTaskTrayAtMinimized;
            set => this.SetProperty(ref this._showInTaskTrayAtMinimized, value);
        }

        [DataMember(Name = "minimize_click_close_button")]
        private bool _minimizeAtCloseButtonClick;
        [IgnoreDataMember]
        public bool MinimizeAtCloseButtonClick
        {
            get => this._minimizeAtCloseButtonClick;
            set => this.SetProperty(ref this._minimizeAtCloseButtonClick, value);
        }

        [DataMember(Name = "system_signout_cancel")]
        private bool _systemCancelSignout;
        [IgnoreDataMember]
        public bool SystemCancelSignout
        {
            get => this._systemCancelSignout;
            set => this.SetProperty(ref this._systemCancelSignout, value);
        }

        [DataMember(Name = "system_shutdown_cancel")]
        private bool _systemCacnelShutdown;
        [IgnoreDataMember]
        public bool SystemCancelShutdown
        {
            get => this._systemCacnelShutdown;
            set => this.SetProperty(ref this._systemCacnelShutdown, value);
        }

        [DataMember(Name = "background_type")]
        private BackgroundType _backgroundType = BackgroundType.None;
        [IgnoreDataMember]
        public BackgroundType BackgroundType
        {
            get => this._backgroundType;
            set => this.SetProperty(ref this._backgroundType, value);
        }

        [DataMember(Name = "background_alignment_x")]
        private AlignmentX _imageAlignmentX;
        [IgnoreDataMember]
        public AlignmentX ImageAlignmentX
        {
            get => this._imageAlignmentX;
            set => this.SetProperty(ref this._imageAlignmentX, value);
        }

        [DataMember(Name = "background_alignment_y")]
        private AlignmentY _imageAlignmentY = AlignmentY.Top;
        [IgnoreDataMember]
        public AlignmentY ImageAlignmentY
        {
            get => this._imageAlignmentY;
            set => this.SetProperty(ref this._imageAlignmentY, value);
        }

        [DataMember(Name = "background_image_stretch")]
        private Stretch _backgroundImageStretch = Stretch.UniformToFill;
        [IgnoreDataMember]
        public Stretch BackgroundImageStretch
        {
            get => this._backgroundImageStretch;
            set => this.SetProperty(ref this._backgroundImageStretch, value);
        }

        [DataMember(Name = "background_image_opacity")]
        private double _backgroundOpacity = 1.0d;
        [IgnoreDataMember]
        public double BackgroundImageOpacity
        {
            get => this._backgroundOpacity;
            set => this.SetProperty(ref this._backgroundOpacity, value);
        }

        [DataMember(Name = "background_image_path")]
        private string _backgroundImagePath;
        [IgnoreDataMember]
        public string BackgroundImagePath
        {
            get => this._backgroundImagePath;
            set => this.SetProperty(ref this._backgroundImagePath, value);
        }

        #endregion

        #region Account

        [DataMember(Name = "account_column_defaults")]
        private NotifiableCollection<ColumnSetting> _defaultColumns;
        [IgnoreDataMember]
        public NotifiableCollection<ColumnSetting> DefaultColumns
        {
            get
            {
                if (this._defaultColumns != null)
                {
                    return this._defaultColumns;
                }
                else
                {
                    var defaultOptions = new[]
                    {
                        new ColumnSetting{ Type = ColumnType.Home },
                        new ColumnSetting{ Type = ColumnType.Notification },
                    };

                    return this._defaultColumns = new NotifiableCollection<ColumnSetting>(defaultOptions);
                }
            }
        }

        #endregion

        #region View

        [DataMember(Name = "timeline_fonts")]
        private string[] _timelineFont;
        [IgnoreDataMember]
        public string[] TimelineFont
        {
            get => this._timelineFont ??= DefaultTimelineFont;
            set => this.SetProperty(ref this._timelineFont, value);
        }

        [DataMember(Name = "timeline_font_size")]
        private double _timelineFontSize;
        [IgnoreDataMember]
        public double TimelineFontSize
        {
            get => this._timelineFontSize;
            set => this.SetProperty(ref this._timelineFontSize, value);
        }

        [DataMember(Name = "timeline_tweet_show_media")]
        private bool _timelineStatusShowMedia = true;
        [IgnoreDataMember]
        public bool TimelineStatusShowMedia
        {
            get => this._timelineStatusShowMedia;
            set => this.SetProperty(ref this._timelineStatusShowMedia, value);
        }

        [DataMember(Name = "timeline_tweet_show_media_detail")]
        private bool _timelineStatusDetailShowMedia = true;
        [IgnoreDataMember]
        public bool TimelineStatusDetailShowMedia
        {
            get => this._timelineStatusDetailShowMedia;
            set => this.SetProperty(ref this._timelineStatusDetailShowMedia, value);
        }

        [DataMember(Name = "timeline_tweet_show_quoted_tweet")]
        private bool _timelineStatusShowQuotedTweet = true;
        [IgnoreDataMember]
        public bool TimelineStatusShowQuotedTweet
        {
            get => this._timelineStatusDetailShowMedia;
            set => this.SetProperty(ref this._timelineStatusShowQuotedTweet, value);
        }

        [DataMember(Name = "timeline_tweet_show_quoted_tweet_detail")]
        private bool _timelineStatusDetailShowQuotedTweet = true;
        [IgnoreDataMember]
        public bool TimelineStatusDetailShowQuotedTweet
        {
            get => this._timelineStatusDetailShowQuotedTweet;
            set => this.SetProperty(ref this._timelineStatusDetailShowQuotedTweet, value);
        }

        [DataMember(Name = "timeline_tweet_show_relative_time")]
        private bool _timelineStatusShowRelativeTime = true;
        [IgnoreDataMember]
        public bool TimelineStatusShowRelativeTime
        {
            get => this._timelineStatusShowRelativeTime;
            set => this.SetProperty(ref this._timelineStatusShowRelativeTime, value);
        }

        [DataMember(Name = "timeline_tweet_show_relatvie_time_detail")]
        private bool _timelineStatusDetailShowRelativeTime;
        [IgnoreDataMember]
        public bool TimelineStatusDetailShowRelativeTime
        {
            get => this._timelineStatusDetailShowRelativeTime;
            set => this.SetProperty(ref this._timelineStatusDetailShowRelativeTime, value);
        }

        [DataMember(Name = "timeline_font_text_rendering")]
        private TextFormattingMode _timelineFontRendering = TextFormattingMode.Display;
        [IgnoreDataMember]
        public TextFormattingMode TimelineFontRendering
        {
            get => this._timelineFontRendering;
            set => this.SetProperty(ref this._timelineFontRendering, value);
        }

        [DataMember(Name = "timeline_status_show_action_button")]
        private bool _timelineStatusActionButtonVisible = true;
        [IgnoreDataMember]
        public bool TimelineStatusActionButtonVsiible
        {
            get => this._timelineStatusActionButtonVisible;
            set => this.SetProperty(ref this._timelineStatusActionButtonVisible, value);
        }

        [DataMember(Name = "timeline_status_detail_show_action_button")]
        private bool _timelineStatusDetailActionButtonVisible = true;
        [IgnoreDataMember]
        public bool TimelineStatusDetailActionButtonVsiible
        {
            get => this._timelineStatusDetailActionButtonVisible;
            set => this.SetProperty(ref this._timelineStatusDetailActionButtonVisible, value);
        }

        [DataMember(Name = "tweet_profile_image_width")]
        private double _tweetProfileImageWidth = DefaultProfileImageWidth;
        [IgnoreDataMember]
        public double TweetProfileImageWidth
        {
            get => this._tweetProfileImageWidth;
            set
            {
                double width = Math.Max(MinimumProfileImageWidth, Math.Min(Math.Floor(value), MaximumProfileImageWidth));
                this.SetProperty(ref this._tweetProfileImageWidth, width);
            }
        }

        [DataMember(Name = "column_width")]
        private double _columnWidth = DefaultColumnWidth;
        [IgnoreDataMember]
        public double ColumnWidth
        {
            get => this._columnWidth;
            set
            {
                double width = Math.Max(MinimumColumnWidth, Math.Min(Math.Floor(value), MaximumColumnWidth));
                this.SetProperty(ref this._columnWidth, width);
            }
        }

        [DataMember(Name = "tweet_show_profile_image")]
        private bool _isShowTweetProfileImage = true;
        [IgnoreDataMember]
        public bool IsShowTweetProfileImage
        {
            get => this._isShowTweetProfileImage;
            set => this.SetProperty(ref this._isShowTweetProfileImage, value);
        }

        [DataMember(Name = "tweet_show_images")]
        private bool _isShowTweetImages = true;
        [IgnoreDataMember]
        public bool IsShowTweetImages
        {
            get => this._isShowTweetImages;
            set => this.SetProperty(ref this._isShowTweetImages, value);
        }

        [DataMember(Name = "tweet_show_quoted_tweet")]
        private bool _isShowTweetQuotedTweet = true;
        [IgnoreDataMember]
        public bool IsShowTweetQuotedTweet
        {
            get => this._isShowTweetQuotedTweet;
            set => this.SetProperty(ref this._isShowTweetQuotedTweet, value);
        }

        [DataMember(Name = "tweet_show_client_name")]
        private bool _isShowTweetClientName = true;
        [IgnoreDataMember]
        public bool IsShowTweetClientName
        {
            get => this._isShowTweetClientName;
            set => this.SetProperty(ref this._isShowTweetClientName, value);
        }

        [DataMember(Name = "tweet_profile_image_form")]
        private ProfileImageForm _profileImageForm = ProfileImageForm.Square;
        [IgnoreDataMember]
        public ProfileImageForm ProfileImageForm
        {
            get => this._profileImageForm;
            set => this.SetProperty(ref this._profileImageForm, value);
        }

        #endregion

        #region Apis

        [DataMember(Name = "mastodon_apis")]
        private NotifiableCollection<ClientKeyCache> _clientKeys;
        [IgnoreDataMember]
        public NotifiableCollection<ClientKeyCache> ClientKeys
        {
            get => this._clientKeys ?? (this._clientKeys = new NotifiableCollection<ClientKeyCache>());
        }

        #endregion

        #region NowPlaying

        [DataMember(Name = "now_playing_format")]
        private string _nowPlayingFormat;
        [IgnoreDataMember]
        public string NowPlayingFormat
        {
            get => this._nowPlayingFormat ??= DefaultNowPlayingFormat;
            set => this.SetProperty(ref this._nowPlayingFormat, value ?? DefaultNowPlayingFormat);
        }

        [DataMember(Name = "now_playing_set_thumbnails")]
        private bool _insertThumbnailAtNowPlaying;
        [IgnoreDataMember]
        public bool InsertThumbnailAtNowPlayying
        {
            get => this._insertThumbnailAtNowPlaying;
            set => this.SetProperty(ref this._insertThumbnailAtNowPlaying, value);
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
            get => this._enableNotification;
            set => this.SetProperty(ref this._enableNotification, value);
        }

        [DataMember(Name = "notification_sound_path")]
        private string _notificationSoundFile;
        [IgnoreDataMember]
        public string NotificationSoundFile
        {
            get => this._notificationSoundFile ??= Defaults.DefaultSoundFile;
            set => this.SetProperty(ref this._notificationSoundFile, value);
        }

        [DataMember(Name = "notification_sound_enable")]
        private bool _enableSoundNotification;
        [IgnoreDataMember]
        public bool EnableSoundNotification
        {
            get => this._enableSoundNotification;
            set => this.SetProperty(ref this._enableSoundNotification, value);
        }

        [DataMember(Name = "notification_popup_enabled")]
        private bool _enablePopupNotification;
        [IgnoreDataMember]
        public bool EnablePopupNotification
        {
            get => this._enablePopupNotification;
            set => this.SetProperty(ref this._enablePopupNotification, value);
        }

        private bool GetIsNotifyEventEnable(NotifyCode code, bool defaultValue)
        {
            return this._ne.GetOrAdd(code, defaultValue);
        }

        private void SetNotifyEventEnable(NotifyCode code, bool value)
        {
            this._ne[code] = value;
        }

        [DataMember(Name = "notification_reply")]
        public bool Notification_Reply
        {
            get => this.GetIsNotifyEventEnable(NotifyCode.Reply, true);
            set => this.SetNotifyEventEnable(NotifyCode.Reply, value);
        }

        [DataMember(Name = "notification_favorite")]
        public bool Notification_Favorite
        {
            get => this.GetIsNotifyEventEnable(NotifyCode.Favorite, true);
            set => this.SetNotifyEventEnable(NotifyCode.Favorite, value);
        }

        [DataMember(Name = "notification_quoted_tweet")]
        public bool Notification_QuotedTweet
        {
            get => this.GetIsNotifyEventEnable(NotifyCode.QuotedTweet, true);
            set => this.SetNotifyEventEnable(NotifyCode.QuotedTweet, value);
        }

        [DataMember(Name = "notification_retweet")]
        public bool Notification_Retweet
        {
            get => this.GetIsNotifyEventEnable(NotifyCode.Retweet, true);
            set => this.SetNotifyEventEnable(NotifyCode.Retweet, value);
        }

        [DataMember(Name = "notification_retweeted_retweet")]
        public bool Notification_RetweetedRetweet
        {
            get => this.GetIsNotifyEventEnable(NotifyCode.RetweetedRetweet, true);
            set => this.SetNotifyEventEnable(NotifyCode.RetweetedRetweet, value);
        }

        [DataMember(Name = "notification_favorited_retweet")]
        public bool Notification_FavoritedRetweet
        {
            get => this.GetIsNotifyEventEnable(NotifyCode.FavoritedRetweet, true);
            set => this.SetNotifyEventEnable(NotifyCode.FavoritedRetweet, value);
        }

        [DataMember(Name = "notification_list_member_added")]
        public bool Notification_ListMemberAdded
        {
            get => this.GetIsNotifyEventEnable(NotifyCode.ListMemberAdded, true);
            set => this.SetNotifyEventEnable(NotifyCode.ListMemberAdded, value);
        }

        [DataMember(Name = "notification_follow")]
        public bool Notification_Follow
        {
            get => this.GetIsNotifyEventEnable(NotifyCode.Follow, true);
            set => this.SetNotifyEventEnable(NotifyCode.Follow, value);
        }

        [DataMember(Name = "notification_dm_received")]
        public bool Notification_DirectMessageReceived
        {
            get => this.GetIsNotifyEventEnable(NotifyCode.DirectMessageCreated, true);
            set => this.SetNotifyEventEnable(NotifyCode.DirectMessageCreated, value);
        }

        #endregion

        #region Mute

        [DataMember(Name = "mute")]
        private NotifiableCollection<Mute> _mute;
        [IgnoreDataMember]
        public NotifiableCollection<Mute> Mute => this._mute ??= new NotifiableCollection<Mute>();

        #endregion

        #region Post

        [DataMember(Name = "post_close_window")]
        private bool _closeWindowAfterPostComplated;
        [IgnoreDataMember]
        public bool CloseWindowAfterPostComplated
        {
            get => this._closeWindowAfterPostComplated;
            set => this.SetProperty(ref this._closeWindowAfterPostComplated, value);
        }

        [DataMember(Name = "post_reply_include_others")]
        private bool _includeOthersAtReply;
        [IgnoreDataMember]
        public bool IncludeOtherAtReply
        {
            get => this._includeOthersAtReply;
            set => this.SetProperty(ref this._includeOthersAtReply, value);
        }

        [DataMember(Name = "post_default_notice_sensitive_media")]
        private bool _noticePostSensitiveMedia;
        [IgnoreDataMember]
        public bool NoticePostSensitiveMedia
        {
            get => this._noticePostSensitiveMedia;
            set => this.SetProperty(ref this._noticePostSensitiveMedia, value);
        }

        #endregion

        #region Network

        [DataMember(Name = "network_system_proxy")]
        private bool _useSystemProxy;
        [IgnoreDataMember]
        public bool UseSystemProxy
        {
            get => this._useSystemProxy;
            set => this.SetProperty(ref this._useSystemProxy, value);
        }

        #endregion

        #region Window

        [DataMember(Name = "window")]
        private WindowSettings _window;
        [IgnoreDataMember]
        public WindowSettings Window => this._window ??= new WindowSettings();

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
