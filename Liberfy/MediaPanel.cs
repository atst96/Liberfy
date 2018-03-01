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
            get => (double)GetValue(HeightCoreProperty);
            set => SetValue(HeightCoreProperty, value);
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

            if (!isWidthInfinity && !isHeightInfinity)
            {
                return availableSize;
            }
            else if (isWidthInfinity && isHeightInfinity)
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
            var children = this.Children;

            double containerWidth = finalSize.Width;
            double containerHeight = finalSize.Height;

            int childrenCount = this.Children.Count;

            if (childrenCount == 1)
            {
                children[0].Arrange(new Rect(0, 0, containerWidth, containerHeight));
            }
            else if (childrenCount == 2)
            {
                double halfWidth = containerWidth / 2.0d;

                children[0].Arrange(new Rect(0, 0, halfWidth, containerHeight));
                children[1].Arrange(new Rect(halfWidth, 0, halfWidth, containerHeight));
            }
            else if (childrenCount > 0)
            {
                double leftItemWidth = containerWidth * (1 / 3d * 2);
                double rightItemWidth = containerWidth - leftItemWidth;

                int rightItemCount = Math.Min(MaxItems, childrenCount) - 1;
                double rightItemHeight = containerHeight / rightItemCount;

                var rightItemSize = new Size(containerWidth - leftItemWidth, rightItemHeight);

                children[0].Arrange(new Rect(0, 0, leftItemWidth, containerHeight));

                for (int i = 1; i < children.Count; ++i)
                {
                    var element = children[i];
                    var point = new Point(leftItemWidth, (i - 1) * rightItemHeight);
                    element.Measure(rightItemSize);
                    element.Arrange(new Rect(point, rightItemSize));
                }
            }

            return new Size(containerWidth, containerHeight);
        }
    }
}
