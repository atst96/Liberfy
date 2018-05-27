using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Twitter.Text;
using SocialApis.Twitter;

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
                status.Text,
                status.Entities.GetAllEntities(),
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
                user.Description,
                user.Entities.Description.GetAllEntities(),
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
                user.Url,
                user.Entities.Url.GetAllEntities(),
                textBlock.Inlines);
        }

        private static async Task SetHyperlinkToPlainText(string plainText, IEnumerable<EntityBase> allEntities, InlineCollection inlines)
        {
            var entities = allEntities
                .OrderBy(entity => entity.IndexStart)
                .GetEnumerator();

            inlines.Clear();

            if (!entities.MoveNext())
            {
                if (!string.IsNullOrEmpty(plainText))
                {
                    inlines.Add(plainText);
                }
            }
            else
            {
                var textReader = new SequentialSurrogateTextReader(plainText);

                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    // リンク付きツイートの作成
                    // ([PlainText])[Link][PlainText][Link][PlainText]....[Link]([PlainText]) の順に生成する

                    var entity = entities.Current;

                    if (entity.IndexStart != 0)
                        inlines.Add(textReader.ReadLength(entity.IndexStart));

                    while (entity != null)
                    {
                        inlines.Add(CreateHyperlink(entity, textReader));

                        int prevEntityIndexEnd = entity.IndexEnd;
                        //if (textBeginPos <= textReader.Length)
                        //{
                        // 次のエンティティ
                        entity = entities.MoveNext() ? entities.Current : null;

                        if (entity == null)
                        {
                            inlines.Add(textReader.ReadToEnd());
                            break;
                        }
                        else
                        {
                            inlines.Add(textReader.ReadLength(entity.IndexStart - prevEntityIndexEnd));
                        }
                        //}
                        //else
                        //    break;

                    }
                });

                textReader = null;
            }
        }

        private static Hyperlink CreateHyperlink(EntityBase entity, SequentialSurrogateTextReader text)
        {
            var link = new Hyperlink()
            {
                Cursor = Cursors.Hand,
                CommandParameter = entity,
            };

            int length = entity.IndexEnd - entity.IndexStart;

            switch (entity)
            {
                case UserMentionEntity mention:
                    link.Inlines.Add(text.ReadLength(length));
                    break;

                case MediaEntity media:
                    link.Inlines.Add(media.DisplayUrl);
                    text.SkipLength(length);
                    break;

                case UrlEntity url:
                    link.Inlines.Add(url.DisplayUrl);
                    text.SkipLength(length);
                    break;

                case HashtagEntity symbol:
                    link.Inlines.Add(text.ReadLength(length));
                    break;

                default:
                    text.SkipLength(length);
                    break;
            }

            return link;
        }
    }
}
