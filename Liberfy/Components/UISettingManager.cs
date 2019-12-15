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

        /// <summary>
        /// [リツイート]のリボン色
        /// </summary>
        public Brush Retweet { get; private set; }

        /// <summary>
        /// [いいね]のリボン色
        /// </summary>
        public Brush Favorite { get; private set; }

        /// <summary>
        /// [リツイート]と[いいね]のリボン色
        /// </summary>
        public Brush RetweetFavorite { get; private set; }

        /// <summary>
        /// UISettingManagerを作成する。
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="setting">設定</param>
        public UISettingManager(App app, Setting setting)
        {
            this._app = app;
            this._setting = setting;

            this.Retweet = this.GetResource<Brush>("Brush.Retweet");
            this.Favorite = this.GetResource<Brush>("Brush.Favorite");
            this.RetweetFavorite = this.GetResource<Brush>("Brush.RetweetFavorite");
        }

        /// <summary>
        /// リソースから指定キーの値を取得する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        public T GetResource<T>(object resourceKey)
        {
            return this._app.TryFindResource<T>(resourceKey);
        }

        /// <summary>
        /// リソースに指定キーに値をセットする。
        /// </summary>
        /// <param name="resourceKey">キー</param>
        /// <param name="value">値</param>
        /// <returns>値が更新されたかどうかを返す</returns>
        private bool TrySetResource(object resourceKey, object value)
        {
            if (object.Equals(this._app.TryFindResource<Brush>(resourceKey), value))
            {
                return false;
            }

            this._app.Resources[resourceKey] = value;

            return true;
        }

        private ProfileImageForm? _profileImageForm;
        private double? _profileImageWidth;

        /// <summary>
        /// UI設定を反映する。
        /// </summary>
        public void Apply()
        {
            var setting = this._setting;

            var oldProfileImageForm = this._profileImageForm;
            double? oldProfileImageWidth = this._profileImageWidth;

            var newProfileImageForm = setting.ProfileImageForm;
            double newProfileImageWidth = setting.TweetProfileImageWidth;

            if (oldProfileImageForm != newProfileImageForm || oldProfileImageWidth != newProfileImageWidth)
            {
                var imageClip = CreateProfileImageMaskGeometry(newProfileImageForm, newProfileImageWidth);

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

        /// <summary>
        /// プロフィール画像のマスクを作成する。
        /// </summary>
        /// <param name="form">画像の表示形状</param>
        /// <param name="width">画像のサイズ</param>
        /// <returns></returns>
        private static Geometry CreateProfileImageMaskGeometry(ProfileImageForm form, double width)
        {
            return form switch
            {
                ProfileImageForm.RoundedCorner => CreateRoundedCornerClip(3.0d, width),
                ProfileImageForm.Ellipse => CreateEllipseClip(width),
                _ => null,
            };
        }

        /// <summary>
        /// 角丸のクリップを作成する。
        /// </summary>
        /// <param name="cornerRadius">角丸のサイズ</param>
        /// <param name="width">クリップのサイズ</param>
        /// <returns>RectangleGeometry</returns>
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
                },
            };
        }

        /// <summary>
        /// 円のクリップを作成する。
        /// </summary>
        /// <param name="width">クリップのサイズ</param>
        /// <returns>EllipseGeometry</returns>
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
