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
                .ToArray();

            inlines.Clear();

            if (entities.Length == 0)
            {
                if (!string.IsNullOrEmpty(plainText))
                {
                    inlines.Add(plainText);
                }
            }
            else
            {
                var text = new TwStringInfo(plainText);
                int textLength = text.Length;

                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    // リンク付きツイートの作成
                    // ([PlainText])[Link][PlainText][Link][PlainText]....[Link]([PlainText]) の順に生成する

                    int endIndex;
                    var entity = entities[0];

                    if (entity.IndexStart != 0)
                    {
                        inlines.Add(text.Slice(0, entity.IndexStart));
                    }

                    for (int i = 0; i < entities.Length; i++)
                    {
                        entity = entities[i];

                        inlines.Add(CreateHyperlink(entity, text));

                        endIndex = entity.IndexEnd;
                        if (endIndex <= textLength)
                        {
                            if (entities.Length > i + 1)
                                inlines.Add(text.Slice(endIndex, entities[i + 1].IndexStart));
                            else
                                inlines.Add(text.Slice(endIndex, textLength));
                        }
                        else
                            break;
                    }
                });

                text = null;
            }
        }

        private static Hyperlink CreateHyperlink(EntityBase entity, TwStringInfo text)
        {
            var link = new Hyperlink()
            {
                Cursor = Cursors.Hand,
                CommandParameter = entity,
            };

            switch (entity)
            {
                case UserMentionEntity mention:
                    link.Inlines.Add(text.Slice(mention.IndexStart, mention.IndexEnd));
                    break;

                case MediaEntity media:
                    link.Inlines.Add(media.DisplayUrl);
                    break;

                case UrlEntity url:
                    link.Inlines.Add(url.DisplayUrl);
                    break;

                case HashtagEntity symbol:
                    link.Inlines.Add(text.Slice(symbol.IndexStart, symbol.IndexEnd));
                    break;
            }

            return link;
        }
    }
}
