using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
    internal class MediaPanel : Panel
    {
        public MediaPanel() : base()
        {
        }

        public const int MaxItems = 6;

        public double Spacing
        {
            get => (double)this.GetValue(SpacingProperty);
            set => this.SetValue(SpacingProperty, value);
        }

        // Using a DependencyProperty as the backing store for Spacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(MediaPanel),
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));


        public double HeightCore
        {
            get => (double)GetValue(HeightCoreProperty);
            set => SetValue(HeightCoreProperty, value);
        }

        public static readonly DependencyProperty HeightCoreProperty =
            DependencyProperty.Register(nameof(HeightCore),
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
            bool isWidthInfinity = double.IsInfinity(availableSize.Width);
            bool isHeightInfinity = double.IsInfinity(availableSize.Height);

            if (isWidthInfinity)
                if (isHeightInfinity)
                    return base.MeasureOverride(availableSize);
                else
                    availableSize.Width = availableSize.Height / HeightCore;

            else if (isHeightInfinity)
                availableSize.Height = availableSize.Width * HeightCore;

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (finalSize.Width <= 0 || finalSize.Height <= 0)
                return finalSize;

            var children = this.Children;
            int childrenCount = children.Count;

            double spacingWidth = this.Spacing;

            if (childrenCount == 1)
            {
                var rect = new Rect(new Point(), finalSize);

                ApplyLayout(children[0], ref rect);
            }
            else if (childrenCount == 2)
            {
                double halfSpacingWidth = spacingWidth / 2d;

                var itemRect = new Rect(
                    0,
                    0,
                    (finalSize.Width / 2) - halfSpacingWidth,
                    finalSize.Height
                );

                ApplyLayout(children[0], ref itemRect);

                itemRect.X += itemRect.Width + spacingWidth;

                ApplyLayout(children[1], ref itemRect);
            }
            else if (childrenCount > 0)
            {
                const double leftItemWidthCoe = 2 / 3d;
                double halfSpacingWidth = spacingWidth / 2d;

                var leftItemRect = new Rect(
                    0,
                    0,
                    (finalSize.Width * leftItemWidthCoe) - halfSpacingWidth,
                    finalSize.Height
                );

                ApplyLayout(children[0], ref leftItemRect);

                int rightItemsCount = childrenCount - 1;

                var rightItemRect = new Rect(
                    leftItemRect.Width + spacingWidth,
                    0,
                    finalSize.Width - leftItemRect.Width - spacingWidth,
                    ((finalSize.Height - (spacingWidth * (rightItemsCount - 1))) / rightItemsCount)
                );

                var rightItemVector = new Vector(
                    0,
                    rightItemRect.Height + spacingWidth
                );

                for (int i = 1; i <= rightItemsCount; ++i)
                {
                    ApplyLayout(children[i], ref rightItemRect);
                    rightItemRect.Location += rightItemVector;
                }
            }

            return finalSize;
        }
    }
}
