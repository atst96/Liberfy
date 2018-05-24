using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Liberfy
{
    public class DropdownButton : ToggleButton
    {
        public DropdownButton()
        {
            this.Checked += this.DropdownButton_Checked;
            this.ContentTemplate = GetButtonTemplate(this);
            this.Style = (Style)this.FindResource(ToolBar.ToggleButtonStyleKey);
        }

        private static DataTemplate GetButtonTemplate(FrameworkElement parent)
        {
            var template = new DataTemplate(typeof(DropdownButton));

            var path = new FrameworkElementFactory(typeof(Path));
            path.SetValue(Path.DataProperty, Geometry.Parse("F1 M 301.14,-189.041L 311.57,-189.041L 306.355,-182.942L 301.14,-189.041 Z"));
            path.SetValue(FrameworkElement.MarginProperty, new Thickness(4, 4, 0, 4));
            path.SetValue(FrameworkElement.WidthProperty, 5d);
            path.SetValue(Shape.FillProperty, Brushes.Black);
            path.SetValue(Shape.StretchProperty, Stretch.Uniform);
            path.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            path.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);

            var panel = new FrameworkElementFactory(typeof(StackPanel));
            panel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));

            var binding = new Binding("Content") { Source = parent };
            contentPresenter.SetBinding(ContentControl.ContentProperty, binding);

            panel.AppendChild(contentPresenter);
            panel.AppendChild(path);

            template.VisualTree = panel;
            return template;
        }

        private void DropdownMenu_Closed(object sender, System.Windows.RoutedEventArgs e)
        {
            this.IsChecked = false;
        }

        private ContextMenu _dropdownMenu;
        public ContextMenu DropdownMenu
        {
            get
            {
                return this._dropdownMenu;
            }
            set
            {
                this._dropdownMenu = value;
                this._dropdownMenu.Closed += DropdownMenu_Closed;
            }
        }

        private void DropdownButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DropdownMenu.PlacementTarget = sender as ToggleButton;
            this.DropdownMenu.Placement = PlacementMode.Bottom;
            this.DropdownMenu.IsOpen = true;
        }
    }
}