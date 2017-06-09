using CoreTweet;
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

namespace Liberfy.Behaviors
{
	internal static class TimelineBehavior
	{
		private static readonly Validator tweetValidator = new Validator();

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

		private static void StatusInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var textBlock = d as TextBlock;
			if (textBlock == null) return;

			textBlock.Inlines.Clear();

			var status = e.NewValue as StatusInfo;
			if (status == null) return;

			var entities = status.GetEntities()
				.OrderBy(entity => entity.Indices[0])
				.ToArray();

			var text = status.Text;
			int textLength = tweetValidator.GetTweetLength(text);

			var inlines = textBlock.Inlines;

			if (entities.Length == 0)
			{
				inlines.Add(text);
			}
			else
			{
				// リンク付きツイートの作成
				// ([テキスト])[リンク] [テキスト] [リンク] [テキスト]....[リンク] [テキスト] の順に生成する

				int endIndex;
				Entity currentEntity = entities[0];

				if (currentEntity.GetStartIndex() != 0)
					inlines.Add(text.Slice(0, currentEntity.GetStartIndex()));

				for (int i = 0; i < entities.Length; i++)
				{
					currentEntity = entities[i];

					inlines.Add(CreateHyperlink(currentEntity, text));

					endIndex = currentEntity.GetEndIndex();
					if (endIndex != textLength)
					{
						if (entities.Length > i + 1)
						{
							inlines.Add(text.Slice(endIndex, currentEntity.GetEndIndex()));
						}
						else
						{
							inlines.Add(text.Slice(endIndex, textLength));
						}
					}
				}
			}
		}

		private static Hyperlink CreateHyperlink(Entity entity, string text)
		{
			var link = new Hyperlink()
			{
				Cursor = Cursors.Hand,
				CommandParameter = entity,
			};

			if (entity is UserMentionEntity mention)
			{
				link.Inlines.Add(text.Slice(mention.Indices[0], mention.Indices[0] + 1) + mention.ScreenName);
			}
			else if (entity is UrlEntity url)
			{
				link.Inlines.Add(url.DisplayUrl);
			}
			else if (entity is SymbolEntity symbol)
			{
				link.Inlines.Add(text.Slice(symbol.Indices[0], symbol.Indices[0] + 1) + symbol.Text);
			}
			else if (entity is MediaEntity media)
			{
				link.Inlines.Add(media.MediaUrl);
			}

			return link;
		}
	}
}
