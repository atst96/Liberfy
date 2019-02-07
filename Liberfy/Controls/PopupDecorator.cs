using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            DependencyProperty.Register("IsRightBalloon", typeof(bool), typeof(PopupDecorator), new PropertyMetadata(false));
    }
}
