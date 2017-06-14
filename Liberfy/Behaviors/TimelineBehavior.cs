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
					// ([テキスト])[リンク][テキスト][リンク][テキスト]....[リンク]([テキスト]) の順に生成する

					int endIndex;
					var entity = entities[0];

					if (entity.StartIndex != 0)
					{
						inlines.Add(text.Slice(0, entity.StartIndex));
					}

					for (int i = 0; i < entities.Length; i++)
					{
						entity = entities[i];

						inlines.Add(CreateHyperlink(entity, text));

						endIndex = entity.EndIndex;
						if (endIndex <= textLength)
						{
							if (entities.Length > i + 1)
								inlines.Add(text.Slice(endIndex, entities[i + 1].StartIndex));
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

		private static Hyperlink CreateHyperlink(Entity entity, TwStringInfo text)
		{
			var link = new Hyperlink()
			{
				Cursor = Cursors.Hand,
				CommandParameter = entity,
			};

			switch (entity)
			{
				case UserMentionEntity mention:
					link.Inlines.Add(text.Slice(mention.StartIndex, mention.EndIndex));
					break;

				case MediaEntity media:
					link.Inlines.Add(media.DisplayUrl);
					break;

				case UrlEntity url:
					link.Inlines.Add(url.DisplayUrl);
					break;

				case SymbolEntity symbol:
					link.Inlines.Add(text.Slice(symbol.StartIndex, symbol.EndIndex));
					break;
			}

			return link;
		}
	}
}
