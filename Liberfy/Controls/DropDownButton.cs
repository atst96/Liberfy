using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Liberfy
{
    internal sealed class DropDownButton : ToggleButton
    {
        static DropDownButton()
        {
            ClickModeProperty.OverrideMetadata(
                typeof(DropDownButton),
                new FrameworkPropertyMetadata(ClickMode.Press));
        }

        private readonly Popup _popup;

        public DropDownButton() : base()
        {
            this._popup = new Popup
            {
                AllowsTransparency = true,
                CustomPopupPlacementCallback = this.GetPopupPosition,
                StaysOpen = false,
                PlacementTarget = this,
            };

            this.SetPopupBinding(Popup.UseLayoutRoundingProperty, "UseLayoutRounding");
            this.SetPopupBinding(Popup.IsOpenProperty, "IsChecked");
            this.SetPopupBinding(Popup.StyleProperty, "PopupStyle");

            this._popup.Opened += this.OnPopupOpened;
            this._popup.Closed += this.OnPopupClosed;
            this._popup.PreviewKeyDown += this.OnPopupKeyDown;

            var routingEvent = new RoutedEventHandler(this.OnClickEventRouted);

            EventManager.RegisterClassHandler(typeof(Button), ButtonBase.ClickEvent, routingEvent);
            EventManager.RegisterClassHandler(typeof(MenuItem), MenuItem.ClickEvent, routingEvent);
            EventManager.RegisterClassHandler(typeof(ListBoxItem), ListBoxItem.PreviewMouseUpEvent, routingEvent);
        }

        private void SetPopupBinding(DependencyProperty property, string path)
        {
            this._popup.SetBinding(property, new Binding(path) { Source = this });
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.ClosePopup();
            }

            base.OnKeyDown(e);
        }

        private void OnPopupKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.ClosePopup();
            }
        }

        private void OnClickEventRouted(object sender, RoutedEventArgs e)
        {
            this.ClosePopup();
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            this.IsHitTestVisible = false;
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            this.IsHitTestVisible = true;
        }

        private CustomPopupPlacement[] GetPopupPosition(Size popupSize, Size targetSize, Point offset)
        {
            double y = targetSize.Height + offset.Y;

            double x = this.IsPopupPositionRight
                ? targetSize.Width - popupSize.Width - offset.X
                : offset.X;

            return new[]
            {
                new CustomPopupPlacement(new Point(x, y), PopupPrimaryAxis.None)
            };
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            this.ShowPopup();

            base.OnChecked(e);
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            this.ClosePopup();

            base.OnUnchecked(e);
        }

        private void ShowPopup()
        {
            this._popup.Placement = PlacementMode.Custom;
            this._popup.IsOpen = true;
            this._popup.Focus();
        }

        private void ClosePopup()
        {
            this._popup.IsOpen = false;
        }

        public bool IsPopupPositionRight
        {
            get => (bool)this.GetValue(IsPopupPositionRightProperty);
            set => this.SetValue(IsPopupPositionRightProperty, value);
        }

        public static readonly DependencyProperty IsPopupPositionRightProperty =
            DependencyProperty.Register("IsPopupPositionRight", typeof(bool), typeof(DropDownButton), new PropertyMetadata(false));

        public Brush PopupBackground
        {
            get => (Brush)this.GetValue(PopupBackgroundProperty);
            set => this.SetValue(PopupBackgroundProperty, value);
        }

        public static readonly DependencyProperty PopupBackgroundProperty =
            DependencyProperty.Register("PopupBackground", typeof(Brush), typeof(DropDownButton), new PropertyMetadata(null));

        public Style PopupStyle
        {
            get => (Style)this.GetValue(PopupStyleProperty);
            set => this.SetValue(PopupStyleProperty, value);
        }

        public static readonly DependencyProperty PopupStyleProperty =
            DependencyProperty.Register("PopupStyle", typeof(Style), typeof(DropDownButton), new PropertyMetadata(null));

        public UIElement PopupContent
        {
            get => (UIElement)this.GetValue(PopupContentProperty);
            set => this.SetValue(PopupContentProperty, value);
        }

        public static readonly DependencyProperty PopupContentProperty =
            DependencyProperty.Register("PopupContent", typeof(UIElement), typeof(DropDownButton),
                new PropertyMetadata(null, new PropertyChangedCallback(OnPopupContentChanged)));

        public Point PopupOffset
        {
            get => (Point)this.GetValue(PopupOffsetProperty);
            set => this.SetValue(PopupOffsetProperty, value);
        }

        public static readonly DependencyProperty PopupOffsetProperty =
            DependencyProperty.Register("PopupOffset", typeof(Point), typeof(DropDownButton),
                new PropertyMetadata(default(Point), new PropertyChangedCallback(OnPopupOffsetChanged)));

        private static void OnPopupOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DropDownButton button && e.NewValue is Point offset)
            {
                var popup = button._popup;

                popup.HorizontalOffset = offset.X;
                popup.VerticalOffset = offset.Y;
            }
        }

        private static void OnPopupContentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DropDownButton button)
            {
                var popup = button._popup;

                popup.Child = args.NewValue as UIElement;
            }
        }
    }
}
