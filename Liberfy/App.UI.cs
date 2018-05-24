using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Liberfy
{
    partial class App
    {
        public static class UI
        {
            private static ProfileImageForm? _previousProfileImageForm = null;
            private static double? _previousProfileImageWidth = null;

            public static void ApplyFromSettings()
            {
                SetResource("UI.Column.Width", Setting.ColumnWidth);
                SetResource("UI.Tweet.ProfileImage.Width", Setting.TweetProfileImageWidth);
                SetResource("UI.Tweet.ProfileImage.Visibility", BoolToVisibility(Setting.IsShowTweetProfileImage));
                SetResource("UI.Tweet.Attachment.Images.Visibility", BoolToVisibility(Setting.IsShowTweetImages));
                SetResource("UI.Tweet.Attachment.QuotedTweet.Visibility", BoolToVisibility(Setting.IsShowTweetQuotedTweet));
                SetResource("UI.Tweet.ClientName.Visibility", BoolToVisibility(Setting.IsShowTweetClientName));

                var profileImageForm = Setting.ProfileImageForm;
                double profileImageWidth = Setting.TweetProfileImageWidth;
                if (_previousProfileImageForm != profileImageForm || _previousProfileImageWidth != profileImageWidth)
                {
                    Geometry clip;
                    switch (profileImageForm)
                    {
                        case ProfileImageForm.RoundedCorner:
                            clip = new RectangleGeometry
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
                            clip = new EllipseGeometry
                            {
                                RadiusX = halfWidth,
                                RadiusY = halfWidth,
                                Center = new Point(halfWidth, halfWidth)
                            };
                            break;

                        default:
                            clip = new RectangleGeometry(new Rect
                            {
                                Width = profileImageWidth,
                                Height = profileImageWidth,
                            });
                            break;
                    }

                    SetResource("UI.Tweet.ProfileImage.Clip", clip);
                    _previousProfileImageForm = profileImageForm;
                    _previousProfileImageWidth = profileImageWidth;
                }
            }
        }
    }
}
