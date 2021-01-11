using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
    internal class PopupDecorator : ContentControl
    {
        public bool IsRightBalloon
        {
            get => (bool)this.GetValue(IsRightBalloonProperty);
            set => this.SetValue(IsRightBalloonProperty, value);
        }

        public static readonly DependencyProperty IsRightBalloonProperty =
            DependencyProperty.Register(nameof(IsRightBalloon), typeof(bool), typeof(PopupDecorator), new(false));
    }
}
