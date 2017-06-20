using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
	public class MediaPanel : Panel
	{
		public MediaPanel() : base()
		{
		}

		public const int MaxItems = 6;

		public double HeightCore
		{
			get { return (double)GetValue(HeightCoreProperty); }
			set { SetValue(HeightCoreProperty, value); }
		}

		public static readonly DependencyProperty HeightCoreProperty =
			DependencyProperty.Register("HeightCore",
				typeof(double), typeof(MediaPanel),
				new FrameworkPropertyMetadata(0.75,
					FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		protected override Size MeasureOverride(Size availableSize)
		{
			double width = availableSize.Width;
			double height = availableSize.Height;
			bool isWidthInfinity = double.IsInfinity(width);
			bool isHeightInfinity = double.IsInfinity(height);

			if (isWidthInfinity && isHeightInfinity)
			{
				return base.MeasureOverride(availableSize);
			}
			else if (isWidthInfinity)
			{
				return new Size(height / HeightCore, height);
			}
			else
			{
				return new Size(width, width * HeightCore);
			}
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			double containerWidth = finalSize.Width;
			double containerHeight = finalSize.Height;

			int childrenCount = this.Children.Count;

			if (childrenCount == 1)
			{
				this.InternalChildren[0].Arrange(new Rect(0, 0, containerWidth, containerHeight));
			}
			else if (childrenCount == 2)
			{
				double halfWidth = containerWidth / 2.0d;

				this.InternalChildren[0].Arrange(new Rect(0, 0, halfWidth, containerHeight));
				this.InternalChildren[1].Arrange(new Rect(halfWidth, 0, halfWidth, containerHeight));
			}
			else if (childrenCount > 0)
			{
				double halfWidth = containerWidth / 2.0d;

				this.InternalChildren[0].Arrange(new Rect(0, 0, halfWidth, containerHeight));

				int rightItems = Math.Min(MaxItems, childrenCount) - 1;
				double rightItemHeight = containerHeight / rightItems;

				for (int i = 0; i < rightItems; i++)
				{
					this.InternalChildren[i + 1].Arrange(
						new Rect(
							halfWidth,
							i * rightItemHeight,
							halfWidth,
							rightItemHeight
						));
				}
			}

			return new Size(containerWidth, containerHeight);
		}
	}
}
