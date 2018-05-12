using System;
using System.Windows.Media;
using static Liberfy.Defines;

namespace Liberfy.ViewModel
{
    partial class SettingWindow
    {
        public double ProfileImageCornerRadius
        {
            get
            {
                switch (_profileImageForm)
                {
                    case ProfileImageForm.RoundedCorner:
                        return 3.0d;

                    case ProfileImageForm.Ellipse:
                        return _previewProfileImageWidth / 2.0d;

                    default:
                        return 0.0d;
                }
            }
        }


        private ProfileImageForm _profileImageForm = App.Setting.ProfileImageForm;
        public ProfileImageForm ProfileImageForm
        {
            get => _profileImageForm;
            set
            {
                if (SetProperty(ref _profileImageForm, value))
                {
                    Setting.ProfileImageForm = value;
                    RaisePropertyChanged(nameof(ProfileImageCornerRadius));
                }
            }
        }

        private bool _isTweetTimestampAbsolute = true;
        public bool IsTweetTimestampAbsolute
        {
            get => _isTweetTimestampAbsolute;
            set
            {
                if (SetProperty(ref _isTweetTimestampAbsolute, value))
                {
                    RaisePropertyChanged(nameof(PreviewTweetTimestampText));
                }
            }
        }

        private bool _isTweetTimestampRelative;
        public bool IsTweetTimestampRelative
        {
            get => _isTweetTimestampRelative;
            set => SetProperty(ref _isTweetTimestampRelative, value);
        }



        private bool _isPreviewTweetRetweeted;
        public bool IsPreviewTweetRetweeted
        {
            get => _isPreviewTweetRetweeted;
            set
            {
                if (SetProperty(ref _isPreviewTweetRetweeted, value))
                {
                    RefreshPreviewTweetActionColor();
                }
            }
        }

        private bool _isPreviewTweetFavorited;
        public bool IsPreviewTweetFavorited
        {
            get => _isPreviewTweetFavorited;
            set
            {
                if (SetProperty(ref _isPreviewTweetFavorited, value))
                {
                    RefreshPreviewTweetActionColor();
                }
            }
        }

        private Brush _previewTweetActionColor = null;
        public Brush PreviewTweetActionColor
        {
            get => _previewTweetActionColor;
            set => SetProperty(ref _previewTweetActionColor, value);
        }

        private void RefreshPreviewTweetActionColor()
        {
            PreviewTweetActionColor = GetTweetReactionColor(
                _isPreviewTweetRetweeted, _isPreviewTweetFavorited);
        }

        public static Brush GetTweetReactionColor(bool isRetweeted, bool isFavorited)
        {
            if (isRetweeted)
            {
                if (isFavorited)
                    return App.Brushes.RetweetFavorite;
                else
                    return App.Brushes.Retweet;
            }
            else if (isFavorited)
            {
                return App.Brushes.Favorite;
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        public string PreviewTweetTimestampText
        {
            get
            {
                if (_isTweetTimestampAbsolute)
                {
                    return "2017年1月1日 0時0分";
                }
                else
                {
                    return "1時間前";
                }
            }
        }



        private double _previewColumnWidth = App.Setting.ColumnWidth;
        public double PreviewColumnWidth
        {
            get => _previewColumnWidth;
            set
            {
                double width = Math.Floor(value);
                if (SetProperty(ref _previewColumnWidth, width))
                {
                    Setting.ColumnWidth = width;
                }
            }
        }


        private double _previewProfileImageWidth = App.Setting.TweetProfileImageWidth;
        public double PreviewProfileImageWidth
        {
            get => _previewProfileImageWidth;
            set
            {
                double width = Math.Floor(value);
                if (SetProperty(ref _previewProfileImageWidth, width))
                {
                    RaisePropertyChanged(nameof(ProfileImageCornerRadius));
                    Setting.TweetProfileImageWidth = width;
                }
            }
        }
    }
}