using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Liberfy.Components
{
    internal class UISettingManager
    {
        private readonly App _app;
        private readonly Setting _setting;

        public Brush Retweet { get; private set; }
        public Brush Favorite { get; private set; }
        public Brush RetweetFavorite { get; private set; }

        public UISettingManager(App app, Setting setting)
        {
            this._app = app;
            this._setting = setting;

            this.Retweet = this.GetResource<Brush>("Brush.Retweet");
            this.Favorite = this.GetResource<Brush>("Brush.Favorite");
            this.RetweetFavorite = this.GetResource<Brush>("Brush.RetweetFavorite");
        }

        public T GetResource<T>(object resourceKey)
        {
            return this._app.TryFindResource<T>(resourceKey);
        }

        private bool TrySetResource(object resourceKey, object value)
        {
            if (object.Equals(this._app.TryFindResource(resourceKey), value))
            {
                return false;
            }

            this._app.Resources[resourceKey] = value;

            return true;
        }

        private ProfileImageForm? _profileImageForm;
        private double? _profileImageWidth;

        public void Apply()
        {
            var setting = this._setting;

            var oldProfileImageForm = this._profileImageForm;
            double? oldProfileImageWidth = this._profileImageWidth;

            var newProfileImageForm = setting.ProfileImageForm;
            double newProfileImageWidth = setting.TweetProfileImageWidth;

            if (oldProfileImageForm != newProfileImageForm || oldProfileImageWidth != newProfileImageWidth)
            {
                var imageClip = CreateProfielImageClip(newProfileImageForm, newProfileImageWidth);

                this.TrySetResource("UI.Tweet.ProfileImage.Clip", imageClip);
                this._profileImageForm = newProfileImageForm;
                this._profileImageWidth = newProfileImageWidth;
            }

            this.TrySetResource("UI.Column.Width", setting.ColumnWidth);
            this.TrySetResource("UI.Tweet.ProfileImage.Width", setting.TweetProfileImageWidth);
            this.TrySetResource("UI.Tweet.ProfileImage.Visibility", ValueConverter.ToVisibility(setting.IsShowTweetProfileImage));
            this.TrySetResource("UI.Tweet.Attachment.Images.Visibility", ValueConverter.ToVisibility(setting.IsShowTweetImages));
            this.TrySetResource("UI.Tweet.Attachment.QuotedTweet.Visibility", ValueConverter.ToVisibility(setting.IsShowTweetQuotedTweet));
            this.TrySetResource("UI.Tweet.ClientName.Visibility", ValueConverter.ToVisibility(setting.IsShowTweetClientName));
        }

        private static Geometry CreateProfielImageClip(ProfileImageForm form, double width)
        {
            switch (form)
            {
                case ProfileImageForm.RoundedCorner:
                    return CreateRoundedCornerClip(3.0d, width);

                case ProfileImageForm.Ellipse:
                    return CreateEllipseClip(width);

                default:
                    return null;
            }
        }

        private static RectangleGeometry CreateRoundedCornerClip(double cornerRadius, double width)
        {
            return new RectangleGeometry
            {
                RadiusX = cornerRadius,
                RadiusY = cornerRadius,
                Rect = new Rect
                {
                    Width = width,
                    Height = width,
                }
            };
        }

        private static EllipseGeometry CreateEllipseClip(double width)
        {
            double cornerRadius = width / 2.0d;

            return new EllipseGeometry
            {
                RadiusX = cornerRadius,
                RadiusY = cornerRadius,
                Center = new Point(cornerRadius, cornerRadius)
            };
        }
    }
}
