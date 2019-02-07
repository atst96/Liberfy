using System;
using System.Windows.Media;
using static Liberfy.Defines;

namespace Liberfy.ViewModel
{
    partial class SettingWindowViewModel
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