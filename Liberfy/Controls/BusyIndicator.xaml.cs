using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Liberfy
{
    /// <summary>
    /// BusyIndicator.xaml の相互作用ロジック
    /// </summary>
    public partial class BusyIndicator : Viewbox
    {
        private bool _isStoryboardLoaded;
        private readonly Storyboard _indicatoryStoryboard;

        static BusyIndicator()
        {
            StretchProperty.OverrideMetadata(typeof(BusyIndicator), new FrameworkPropertyMetadata(Stretch.Uniform));
        }

        /// <summary>
        /// <see cref="BusyIndicator"/>を生成する。
        /// </summary>
        public BusyIndicator()
        {
            this.InitializeComponent();
            this._indicatoryStoryboard = this.TryFindResource("IndicatorStoryboard") as Storyboard;
        }

        /// <summary>
        /// アニメーションを開始する。
        /// </summary>
        private void StartAnimation()
        {
            var storyboard = this._indicatoryStoryboard;
            var indicator = this.indicator;

            if (!this._isStoryboardLoaded)
            {
                storyboard.Begin(indicator, true);
                this._isStoryboardLoaded = true;
            }
            else
            {
                storyboard.Resume(indicator);
            }
        }

        /// <summary>
        /// アニメーションを停止する。
        /// </summary>
        private void StopAnimation()
        {
            this._indicatoryStoryboard.Pause(this.indicator);
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
            DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(BusyIndicator), new(false, IsBusyPropertyChagned));

        /// <summary>
        /// <see cref="IsBusy"/>の変更時
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void IsBusyPropertyChagned(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is BusyIndicator indicator && e.NewValue is bool isAnimate)
            {
                if (isAnimate)
                {
                    indicator.StartAnimation();
                }
                else
                {
                    indicator.StopAnimation();
                }
            }
        }
    }
}
