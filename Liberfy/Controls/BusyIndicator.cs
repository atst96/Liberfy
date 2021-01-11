using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
    /// <summary>
    /// BusyIndicator.xaml の相互作用ロジック
    /// </summary>
    public class BusyIndicator : Control
    {
        static BusyIndicator()
        {
        }

        /// <summary>
        /// <see cref="BusyIndicator"/>を生成する。
        /// </summary>
        public BusyIndicator() : base()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyIndicator), new PropertyMetadata(typeof(BusyIndicator)));
        }

        /// <summary>
        /// アニメーションの表示状態を取得または設定する。
        /// </summary>
        public bool IsBusy
        {
            get => (bool)this.GetValue(IsBusyProperty);
            set => this.SetValue(IsBusyProperty, value);
        }

        /// <summary>
        /// アニメーション表示状態のプロパティ
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(BusyIndicator), new(false));
    }
}
