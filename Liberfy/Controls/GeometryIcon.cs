using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Liberfy
{
    /// <summary>
    /// アイコン
    /// </summary>
    internal class GeometryIcon : Control
    {
        static GeometryIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeometryIcon), new FrameworkPropertyMetadata(typeof(GeometryIcon)));
        }

        /// <summary>
        /// アイコンの描画色を取得または設定する。
        /// </summary>
        public Brush Fill
        {
            get => (Brush)this.GetValue(FillProperty);
            set => this.SetValue(FillProperty, value);
        }

        /// <summary>
        /// <see cref="Fill"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(GeometryIcon), new(null));

        /// <summary>
        /// アイコンデータを取得または設定する。
        /// </summary>
        public Geometry Data
        {
            get => (Geometry)this.GetValue(DataProperty);
            set => this.SetValue(DataProperty, value);
        }

        /// <summary>
        /// <see cref="Data"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(Geometry), typeof(GeometryIcon), new(null));
    }
}
