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

        internal static IEnumerable<EntityBase> GetOrderedEntities(StatusInfo status)
        {
            return new EntityBase[][]
            {
                status.Entities.Hashtags,
                status.Entities.Symbols,
                status.Entities.Urls,
                status.Entities.UserMentions,
                status.Entities.Media
            }.Merge();
        }

        private static async void StatusInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null) return;

            textBlock.Inlines.Clear();

            var status = e.NewValue as StatusInfo;
            if (status == null) return;

            var entities = status.GetEntities()
                .OrderBy(entity => entity.Indices[0])
                .ToArray();

            var text = new TwStringInfo(status.Text);
            int textLength = text.Length;

            var inlines = textBlock.Inlines;

            if (entities.Length == 0)
            {
                inlines.Add(status.Text);
            }
            else
            {
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
            }

            // text.Dispose();
            text = null;
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
