using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Liberfy
{
    internal sealed class TimelineViewItem : ListBoxItem
    {
        private TimelineView _container;
        private IItem _item;
        private bool _isAnimating;

        private static Duration _duration = new Duration(TimeSpan.FromMilliseconds(500));

        public TimelineViewItem() : base()
        {
            this.Loaded += this.TimelineViewItem_Loaded;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            this._item = newContent as IItem;
        }

        private void TimelineViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (this._container.ItemsSource is IEnumerable<IItem> items)
            {
                if (items.FirstOrDefault() == this._item)
                {
                    // var size = this.MeasureOverride(new Size(this._container.ItemWidth, double.PositiveInfinity));
                    var size = this.MeasureCore(new Size(this._container.ItemWidth, double.PositiveInfinity));
                    double height = size.Height;

                    var animation = new DoubleAnimation(0.0d, height, _duration)
                    {
                        EasingFunction = new QuadraticEase(),
                        FillBehavior = FillBehavior.Stop,
                    };
                    animation.Completed += this.Animation_Completed;
                    this._isAnimating = true;
                    this.BeginAnimation(TimelineViewItem.HeightProperty, animation);
                }
            }
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            this._isAnimating = false;
        }

        public bool IsRetweetedItem
        {
            get { return (bool)this.GetValue(TimelineViewItem.IsRetweetedItemProperty); }
            set { this.SetValue(TimelineViewItem.IsRetweetedItemProperty, value); }
        }

        public static readonly DependencyProperty IsRetweetedItemProperty =
            DependencyProperty.Register(nameof(TimelineViewItem.IsRetweetedItem), typeof(bool), typeof(TimelineViewItem), new PropertyMetadata(false));

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (this._container == null)
                this._container = ItemsControl.ItemsControlFromItemContainer(this) as TimelineView;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            constraint.Width = this._container.ItemWidth;
            return base.MeasureOverride(constraint);
        }
    }
}
