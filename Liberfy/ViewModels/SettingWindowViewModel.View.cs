using Liberfy.Components;
using System;
using System.Windows.Media;
using static Liberfy.Defaults;

namespace Liberfy.ViewModels
{
    internal partial class SettingWindowViewModel
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
                        return this._previewProfileImageWidth / 2.0d;

                    default:
                        return 0.0d;
                }
            }
        }


        private ProfileImageForm _profileImageForm = App.Setting.ProfileImageForm;
        public ProfileImageForm ProfileImageForm
        {
            get => this._profileImageForm;
            set
            {
                if (this.RaisePropertyChangedIfSet(ref this._profileImageForm, value))
                {
                    this.Setting.ProfileImageForm = value;
                    this.RaisePropertyChanged(nameof(this.ProfileImageCornerRadius));
                }
            }
        }

        public static Brush GetTweetReactionColor(bool isRetweeted, bool isFavorited)
        {
            if (isRetweeted)
            {
                if (isFavorited)
                {
                    return App.Instance.UIManager.RetweetFavorite;
                }
                else
                {
                    return App.Instance.UIManager.Retweet;
                }
            }
            else if (isFavorited)
            {
                return App.Instance.UIManager.Favorite;
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        private double _previewColumnWidth = App.Setting.ColumnWidth;
        public double PreviewColumnWidth
        {
            get => this._previewColumnWidth;
            set
            {
                double width = Math.Floor(value);
                if (this.RaisePropertyChangedIfSet(ref this._previewColumnWidth, width))
                {
                    this.Setting.ColumnWidth = width;
                }
            }
        }


        private double _previewProfileImageWidth = App.Setting.TweetProfileImageWidth;
        public double PreviewProfileImageWidth
        {
            get => this._previewProfileImageWidth;
            set
            {
                double width = Math.Floor(value);
                if (this.RaisePropertyChangedIfSet(ref this._previewProfileImageWidth, width))
                {
                    this.RaisePropertyChanged(nameof(ProfileImageCornerRadius));
                    this.Setting.TweetProfileImageWidth = width;
                }
            }
        }
    }
}
