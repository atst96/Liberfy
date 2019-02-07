using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Liberfy
{
    /// <summary>
    /// BusyIndicator.xaml の相互作用ロジック
    /// </summary>
    public partial class BusyIndicator : UserControl
    {
        private readonly Storyboard _animator;

        public BusyIndicator()
        {
            InitializeComponent();

            this._animator = this.TryFindResource("animator") as Storyboard;
        }

        private void StartAnimation()
        {
            this.canvas.BeginStoryboard(this._animator);
        }

        private void StopAnimation()
        {
            this._animator.Stop();
        }

        public bool IsBusy
        {
            get => (bool)this.GetValue(IsBusyProperty);
            set => this.SetValue(IsBusyProperty, value);
        }

        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyIndicator),
                new PropertyMetadata(false, IsBusyPropertyChagned));

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
