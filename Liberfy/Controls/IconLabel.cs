using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Liberfy.Controls;

namespace Liberfy
{
    /// <summary>
    /// </summary>
    internal class IconLabel : ContentControl
    {
        static IconLabel()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(IconLabel), new(typeof(IconLabel)));
        }

        public IconLabel() : base()
        {
        }

        /// <summary>
        /// アイコン・ラベルの表示状態を取得または設定する。
        /// </summary>
        public IconLabelVisibility ContentVisibility
        {
            get => (IconLabelVisibility)this.GetValue(ContentVisibilityProperty);
            set => this.SetValue(ContentVisibilityProperty, value);
        }

        /// <summary>
        /// <see cref="ContentVisibility"/> 
        /// </summary>
        public static readonly DependencyProperty ContentVisibilityProperty =
            DependencyProperty.Register(nameof(ContentVisibility), typeof(IconLabelVisibility), typeof(IconLabel), new(IconLabelVisibility.Both));

        /// <summary>
        /// アイコンの幅を取得または設定する。
        /// </summary>
        public double IconWidth
        {
            get => (double)this.GetValue(IconWidthProperty);
            set => this.SetValue(IconWidthProperty, value);
        }

        /// <summary>
        /// <see cref="IconWidth"/>のプロパティ。
        /// </summary>
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(IconLabel), new(double.NaN));

        /// <summary>
        /// アイコンの幅を取得または設定する。
        /// </summary>
        public double IconHeight
        {
            get => (double)this.GetValue(IconHeightProperty);
            set => this.SetValue(IconHeightProperty, value);
        }

        /// <summary>
        /// <see cref="IconHeight"/>のプロパティ。
        /// </summary>
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(IconLabel), new(double.NaN));

        /// <summary>
        /// アイコンとコンテンツの間隔を取得または設定する。
        /// </summary>
        public double IconContentInterval
        {
            get => (double)this.GetValue(IconContentIntervalProperty);
            set => this.SetValue(IconContentIntervalProperty, value);
        }

        /// <summary>
        /// <see cref="IconContentInterval"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty IconContentIntervalProperty =
            DependencyProperty.Register(nameof(IconContentInterval), typeof(double), typeof(IconLabel), new(4.0d));

        /// <summary>
        /// アイコンを取得または設定する。
        /// </summary>
        public Geometry Icon
        {
            get => (Geometry)this.GetValue(IconProperty);
            set => this.SetValue(IconProperty, value);
        }

        /// <summary>
        /// <see cref="Icon"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(Geometry), typeof(IconLabel), new(null));
    }
}
