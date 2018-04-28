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

        private static void ApplyLayout(UIElement element, ref Rect rect)
        {
            element.Measure(rect.Size);
            element.Arrange(rect);
        }

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

            int childrenCount = children.Count;

            if (childrenCount == 1)
            {
                var rect = new Rect(new Point(), finalSize);

                ApplyLayout(children[0], ref rect);
            }
            else if (childrenCount == 2)
            {
                const double splitWidthCoe = 0.5d;

                var itemRect = new Rect(
                    0,
                    0,
                    finalSize.Width * splitWidthCoe,
                    finalSize.Height
                );

                ApplyLayout(children[0], ref itemRect);

                itemRect.X += itemRect.Width;

                ApplyLayout(children[1], ref itemRect);
            }
            else if (childrenCount > 0)
            {
                const double leftItemWidthCoe = 2 / 3d;

                var leftItemRect = new Rect(
                    0,
                    0,
                    finalSize.Width * leftItemWidthCoe,
                    finalSize.Height
                );

                ApplyLayout(children[0], ref leftItemRect);

                int rightItemCount = childrenCount - 1;

                var rightItemRect = new Rect(
                    leftItemRect.Width,
                    0,
                    finalSize.Width - leftItemRect.Width,
                    finalSize.Height / rightItemCount
                );

                var rightItemVec = new Vector(
                    0,
                    rightItemRect.Height
                );

                for (int i = 1; i <= rightItemCount; ++i)
                {
                    ApplyLayout(children[i], ref rightItemRect);
                    rightItemRect.Location += rightItemVec;
                }
            }

            return finalSize;
        }
    }
}
