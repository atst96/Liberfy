using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Liberfy
{
    internal sealed class DropDownMenuButton : ToggleButton
    {
        public DropDownMenuButton() : base()
        {
        }

        private ContextMenu _dropDownMenu;
        public ContextMenu DropDownMenu
        {
            get => (ContextMenu)this.GetValue(DropDownMenuProperty);
            set => this.SetValue(DropDownMenuProperty, value);
        }

        private void RegisterDropDownMenu(ContextMenu dropDownMenu)
        {
            if (object.ReferenceEquals(this._dropDownMenu, dropDownMenu))
            {
                this.UnregisterDropDownMenu();
                return;
            }

            if (dropDownMenu != null)
            {
                var binding = new Binding("IsChecked") { Source = this };
                dropDownMenu.SetBinding(ContextMenu.IsOpenProperty, binding);

                dropDownMenu.PlacementTarget = this;
                dropDownMenu.CustomPopupPlacementCallback = this.GetPopupPosition;
                dropDownMenu.Opened += this.OnDropDownMenuOpened;
                dropDownMenu.Closed += this.OnDropDownMenuClosed;
                dropDownMenu.PreviewKeyDown += this.OnDropDownPreviewKeyDown;

                this._dropDownMenu = dropDownMenu;
            }
        }

        private void UnregisterDropDownMenu()
        {
            var dropDownMenu = this._dropDownMenu;
            if (dropDownMenu != null)
            {
                BindingOperations.ClearBinding(dropDownMenu, ContextMenu.IsOpenProperty);

                dropDownMenu.PlacementTarget = null;
                dropDownMenu.CustomPopupPlacementCallback = null;
                dropDownMenu.Opened -= this.OnDropDownMenuOpened;
                dropDownMenu.Closed -= this.OnDropDownMenuClosed;
                dropDownMenu.PreviewKeyDown -= this.OnDropDownPreviewKeyDown;

                this._dropDownMenu = default;
            }
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            this.ShowDropDownMenu();
            base.OnChecked(e);
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            this.CloseDropDownMenu();
            base.OnUnchecked(e);
        }


        public bool IsMenuPositionRight
        {
            get => (bool)this.GetValue(IsMenuPositionRightProperty);
            set => this.SetValue(IsMenuPositionRightProperty, value);
        }

        public static readonly DependencyProperty IsMenuPositionRightProperty =
            DependencyProperty.Register("IsMenuPositionRight", typeof(bool), typeof(DropDownMenuButton), new PropertyMetadata(false));

        private CustomPopupPlacement[] GetPopupPosition(Size popupSize, Size targetSize, Point offset)
        {
            double y = targetSize.Height + offset.Y;

            double x = this.IsMenuPositionRight
                ? targetSize.Width - popupSize.Width - offset.X
                : offset.X;

            return new[]
            {
                new CustomPopupPlacement(new Point(x, y), PopupPrimaryAxis.None)
            };
        }

        private void OnDropDownMenuOpened(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = false;
        }

        private void OnDropDownMenuClosed(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = true;
        }

        private void ShowDropDownMenu()
        {
            var menu = this._dropDownMenu;
            if (menu != null)
            {
                menu.Placement = PlacementMode.Custom;
                menu.IsOpen = true;
                menu.Focus();
            }
        }

        private void CloseDropDownMenu()
        {
            this._dropDownMenu?.SetValue(ContextMenu.IsOpenProperty, false);
        }

        private void OnDropDownPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.CloseDropDownMenu();
            }
        }

        public static readonly DependencyProperty DropDownMenuProperty =
            DependencyProperty.Register("DropDownMenu", typeof(ContextMenu), typeof(DropDownMenuButton),
                new PropertyMetadata(null, DropDownMenuButton.OnDropDownMenuChanged));

        private static void OnDropDownMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (DropDownMenuButton)d;

            if (e.NewValue == default)
            {
                button.UnregisterDropDownMenu();
            }
            else if (e.NewValue is ContextMenu newMenu)
            {
                button.RegisterDropDownMenu(newMenu);
            }
        }
    }
}
