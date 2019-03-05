using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Liberfy.Components
{
    internal static class UIManager
    {
        private readonly static App App = App.Instance;
        private readonly static Setting Setting = App.Setting;

        public static readonly Brush Retweet = GetResource<Brush>("Brush.Retweet");
        public static readonly Brush Favorite = GetResource<Brush>("Brush.Favorite");
        public static readonly Brush RetweetFavorite = GetResource<Brush>("Brush.RetweetFavorite");

        public static T GetResource<T>(object resourceKey)
        {
            return App.TryFindResource(resourceKey) is T value ? value : default;
        }

        private static bool SetResource(object resourceKey , object value)
        {
            if (object.Equals(App.TryFindResource(resourceKey), value))
            {
                return false;
            }

            App.Resources[resourceKey] = value;

            return true;
        }

        private static Visibility BooleanToVisibility(bool isVisible)
        {
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private static ProfileImageForm? _profileImageForm;
        private static double? _profileImageWidth;

        public static void Apply()
        {
            SetResource("UI.Column.Width", Setting.ColumnWidth);
            SetResource("UI.Tweet.ProfileImage.Width", Setting.TweetProfileImageWidth);
            SetResource("UI.Tweet.ProfileImage.Visibility", BooleanToVisibility(Setting.IsShowTweetProfileImage));
            SetResource("UI.Tweet.Attachment.Images.Visibility", BooleanToVisibility(Setting.IsShowTweetImages));
            SetResource("UI.Tweet.Attachment.QuotedTweet.Visibility", BooleanToVisibility(Setting.IsShowTweetQuotedTweet));
            SetResource("UI.Tweet.ClientName.Visibility", BooleanToVisibility(Setting.IsShowTweetClientName));

            var profileImageForm = Setting.ProfileImageForm;
            double profileImageWidth = Setting.TweetProfileImageWidth;

            if (_profileImageForm != profileImageForm || _profileImageWidth != profileImageWidth)
            {
                Geometry imageClip;

                switch (profileImageForm)
                {
                    case ProfileImageForm.RoundedCorner:
                        imageClip = new RectangleGeometry
                        {
                            RadiusX = 3.0d,
                            RadiusY = 3.0d,
                            Rect = new Rect
                            {
                                Width = profileImageWidth,
                                Height = profileImageWidth,
                            }
                        };
                        break;

                    case ProfileImageForm.Ellipse:
                        double halfWidth = profileImageWidth / 2.0d;
                        imageClip = new EllipseGeometry
                        {
                            RadiusX = halfWidth,
                            RadiusY = halfWidth,
                            Center = new Point(halfWidth, halfWidth)
                        };
                        break;

                    default:
                        imageClip = null;
                        break;
                }

                SetResource("UI.Tweet.ProfileImage.Clip", imageClip);
                _profileImageForm = profileImageForm;
                _profileImageWidth = profileImageWidth;
            }
        }
    }
}
