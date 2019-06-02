using Liberfy.Model;
using SocialApis.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Liberfy.Behaviors
{
    internal static class TimelineBehavior
    {
        public static IStatusInfo GetStatusInfo(DependencyObject obj)
        {
            return (IStatusInfo)obj.GetValue(StatusInfoProperty);
        }

        public static void SetStatusInfo(DependencyObject obj, IStatusInfo value)
        {
            obj.SetValue(StatusInfoProperty, value);
        }

        public static readonly DependencyProperty StatusInfoProperty =
            DependencyProperty.RegisterAttached("StatusInfo",
                typeof(IStatusInfo), typeof(TimelineBehavior),
                new PropertyMetadata(null, StatusInfoChanged));

        private static async void StatusInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock && e.NewValue is IStatusInfo status)
            {
                await SetHyperlinkToPlainText(status.Entities, textBlock.Inlines);
            }
        }


        public static IUserInfo GetUserDescription(DependencyObject obj)
        {
            return (IUserInfo)obj.GetValue(UserDescriptionProperty);
        }

        public static void SetUserDescription(DependencyObject obj, IUserInfo value)
        {
            obj.SetValue(UserDescriptionProperty, value);
        }

        public static readonly DependencyProperty UserDescriptionProperty =
            DependencyProperty.RegisterAttached("UserDescription",
                typeof(IUserInfo), typeof(TimelineBehavior),
                new PropertyMetadata(null, UserDescriptionChanged));

        private static async void UserDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock && e.NewValue is IUserInfo user)
            {
                await SetHyperlinkToPlainText(user.DescriptionEntities, textBlock.Inlines);
            }
        }


        public static IUserInfo GetUserUrl(DependencyObject obj)
        {
            return (IUserInfo)obj.GetValue(UserUrlProperty);
        }

        public static void SetUserUrl(DependencyObject obj, IUserInfo value)
        {
            obj.SetValue(UserUrlProperty, value);
        }

        public static readonly DependencyProperty UserUrlProperty =
            DependencyProperty.RegisterAttached("UserUrl",
                typeof(IUserInfo), typeof(TimelineBehavior), new PropertyMetadata(null, UserUrlChanged));

        private static async void UserUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock && e.NewValue is IUserInfo user)
            {
                await SetHyperlinkToPlainText(user.UrlEntities, textBlock.Inlines);
            }
        }

        private static readonly Binding _fontSizeBinding = new Binding
        {
            Path = new PropertyPath("FontSize"),
            RelativeSource = new RelativeSource
            {
                AncestorType = typeof(TextBlock),
            },
        };

        private static readonly RequestCachePolicy EmojiCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);

        private static BitmapSource ImageSourceFromUri(string url)
        {
            var uri = new Uri(url, UriKind.Absolute);

            return new BitmapImage(uri)
            {
                CreateOptions = BitmapCreateOptions.DelayCreation,
            };
        }

        private static Image ImageFromImageSousrce(ImageSource imageSource)
        {
            var image = new Image
            {
                Margin = new Thickness(.0d, .0d, .0d, .0d),
                Stretch = Stretch.Uniform,
                Source = imageSource,
            };

            image.SetBinding(FrameworkElement.HeightProperty, TimelineBehavior._fontSizeBinding);

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

            return image;
        }

        private static DispatcherOperation SetHyperlinkToPlainText(IEnumerable<IEntity> entities, InlineCollection inlines)
        {
            return Application.Current.Dispatcher.InvokeAsync(() =>
            {
                inlines.Clear();

                foreach (var entity in entities)
                {
                    if (entity is PlainTextEntity)
                    {
                        inlines.Add(entity.DisplayText);
                    }
                    else if (entity is EmojiEntity emojiEntity)
                    {
                        var source = TimelineBehavior.ImageSourceFromUri(emojiEntity.ImageUrl);
                        var imageElement = TimelineBehavior.ImageFromImageSousrce(source);

                        imageElement.ToolTip = emojiEntity.DisplayText;

                        inlines.Add(new InlineUIContainer(imageElement));
                    }
                    else
                    {
                        var link = new Hyperlink()
                        {
                            Cursor = Cursors.Hand,
                            CommandParameter = entity,
                        };

                        link.Inlines.Add(entity.DisplayText);

                        inlines.Add(link);
                    }
                }
            });
        }

        public static IUserInfo GetProfileImage(DependencyObject obj)
        {
            return (IUserInfo)obj.GetValue(ProfileImageProperty);
        }

        public static void SetProfileImage(DependencyObject obj, IUserInfo value)
        {
            obj.SetValue(ProfileImageProperty, value);
        }

        public static readonly DependencyProperty ProfileImageProperty =
            DependencyProperty.RegisterAttached("ProfileImage",
                typeof(IUserInfo), typeof(TimelineBehavior),
                new PropertyMetadata(null, OnProfileImagePropertyChagned));

        private static async void OnProfileImagePropertyChagned(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var image = d as Image;
            if (image == null)
            {
                return;
            }

            if (e.NewValue == null)
            {
                BindingOperations.ClearBinding(image, Image.SourceProperty);
                return;
            }

            if (e.NewValue is IUserInfo userInfo)
            {
                var cacheInfo = App.ProfileImageCache.GetCacheInfo(userInfo);

                var binding = new Binding("Image")
                {
                    Source = cacheInfo,
                    Mode = BindingMode.OneWay,
                };

                image.SetBinding(Image.SourceProperty, binding);
            }
        }
    }
}
