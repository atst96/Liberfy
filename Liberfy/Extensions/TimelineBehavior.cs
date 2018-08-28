using Liberfy.Model;
using SocialApis.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Liberfy.Behaviors
{
    internal static class TimelineBehavior
    {
        public static StatusInfo GetStatusInfo(DependencyObject obj)
        {
            return (StatusInfo)obj.GetValue(StatusInfoProperty);
        }

        public static void SetStatusInfo(DependencyObject obj, StatusInfo value)
        {
            obj.SetValue(StatusInfoProperty, value);
        }

        public static readonly DependencyProperty StatusInfoProperty =
            DependencyProperty.RegisterAttached("StatusInfo",
                typeof(StatusInfo), typeof(TimelineBehavior),
                new PropertyMetadata(null, StatusInfoChanged));

        private static async void StatusInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null) return;

            var status = e.NewValue as StatusInfo;
            if (status == null) return;

            await SetHyperlinkToPlainText(
                status.Entities,
                textBlock.Inlines);
        }


        public static UserInfo GetUserDescription(DependencyObject obj)
        {
            return (UserInfo)obj.GetValue(UserDescriptionProperty);
        }

        public static void SetUserDescription(DependencyObject obj, UserInfo value)
        {
            obj.SetValue(UserDescriptionProperty, value);
        }

        public static readonly DependencyProperty UserDescriptionProperty =
            DependencyProperty.RegisterAttached("UserDescription",
                typeof(UserInfo), typeof(TimelineBehavior),
                new PropertyMetadata(null, UserDescriptionChanged));

        private static async void UserDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null) return;

            var user = e.NewValue as UserInfo;
            if (user == null) return;

            await SetHyperlinkToPlainText(
                user.DescriptionEntities,
                textBlock.Inlines);
        }


        public static UserInfo GetUserUrl(DependencyObject obj)
        {
            return (UserInfo)obj.GetValue(UserUrlProperty);
        }

        public static void SetUserUrl(DependencyObject obj, UserInfo value)
        {
            obj.SetValue(UserUrlProperty, value);
        }

        public static readonly DependencyProperty UserUrlProperty =
            DependencyProperty.RegisterAttached("UserUrl",
                typeof(UserInfo), typeof(TimelineBehavior),
                new PropertyMetadata(null, UserUrlChanged));

        private static async void UserUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null) return;

            var user = e.NewValue as UserInfo;
            if (user == null) return;

            await SetHyperlinkToPlainText(
                user.UrlEntities,
                textBlock.Inlines);
        }
        private static async Task SetHyperlinkToPlainText(IEnumerable<IEntity> entities, InlineCollection inlines)
        {
            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                inlines.Clear();

                foreach (var entity in entities)
                {
                    if (entity is PlainTextEntity)
                    {
                        inlines.Add(entity.DisplayText);
                    }
                    else
                    {
                        var link = new Hyperlink()
                        {
                            Cursor = Cursors.Hand,
                            CommandParameter = entity,
                        };

                        link.Inlines.Add(entity.DisplayText);

                        //switch (entity)
                        //{
                        //    case MentionEntity mention:
                        //        link.Inlines.Add(text);
                        //        break;

                        //    case MediaEntity media:
                        //        link.Inlines.Add(media.DisplayUrl);
                        //        break;

                        //    case UrlEntity url:
                        //        link.Inlines.Add(url.DisplayUrl);
                        //        break;

                        //    case HashtagEntity symbol:
                        //        link.Inlines.Add(text);
                        //        break;

                        //    default:
                        //        throw new NotImplementedException();
                        //}

                        inlines.Add(link);
                    }
                }
            });
        }
    }
}
