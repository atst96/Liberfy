using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shell;

namespace Liberfy
{
    [TemplatePart(Name = "PART_Container", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ActionButtonBorderTop", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_ActionButtonBorderRight", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_MinimizeButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MaximizeButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_RestoreButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_TitleBarPanel", Type = typeof(Panel))]
    [Localizability(LocalizationCategory.Ignore)]
    public class CustomWindow : Window
    {
        private double _borderPadWidth = 4;
        private double _resizeBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
        private readonly Thickness _maximizedBorderMargin;
        private double _scale;

        private WindowChrome _chrome;
        private Border _container;
        private FrameworkElement _actionButtonBorderTop;
        private FrameworkElement _actionButtonBorderRight;
        private Button _minimizeButton;
        private Button _maximizeButton;
        private Button _restoreButton;
        private Button _closeButton;
        private Panel _titleBarPanel;

        static CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));
        }

        public CustomWindow() : base()
        {
            var chrome = new WindowChrome
            {
                UseAeroCaptionButtons = false,
                CornerRadius = new CornerRadius(0.0),
                GlassFrameThickness = new Thickness(1.0),
            };

            this._chrome = chrome;
            WindowChrome.SetWindowChrome(this, chrome);

            var commandBindings = new List<CommandBinding>
            {
                new CommandBinding(SystemCommands.MinimizeWindowCommand, ExecuteMinimizeCommand),
                new CommandBinding(SystemCommands.MaximizeWindowCommand, ExecuteMaximizeCommand, CanExecuteMaximizeCommand),
                new CommandBinding(SystemCommands.RestoreWindowCommand, ExecuteRestoreCommand),
                new CommandBinding(SystemCommands.CloseWindowCommand, ExecuteCloseCommand),
            };

            this.CommandBindings.AddRange(commandBindings);

            this._maximizedBorderMargin = new Thickness(this._resizeBorderWidth + this._borderPadWidth);

            this.SetScale(VisualTreeHelper.GetDpi(this));
        }

        #region CommandBindign Methods

        private void ExecuteMinimizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void ExecuteMaximizeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void CanExecuteMaximizeCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void ExecuteRestoreCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void ExecuteCloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        #endregion

        public void SetScale(in DpiScale dpi)
        {
            this._scale = dpi.DpiScaleX;
        }

        private void SetCaptionHeight(double height)
        {
            this._chrome.CaptionHeight = height;
        }

        private void SetPadding(Thickness thickness)
        {
            this._container?.SetValue(Border.PaddingProperty, Rescale(thickness, this._scale));
        }

        private void SetTitleBarForeground(Brush textBrush)
        {
            this._titleBarPanel?.SetValue(TextBlock.ForegroundProperty, textBrush);
        }

        private void SetWindowBorderThickness(in Thickness thickness)
        {
            var scaledThickness = Rescale(thickness, this._scale);
            this._container?.SetValue(Border.BorderThicknessProperty, scaledThickness);

            double width = 0.0d, height = 0.0d;

            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                width = scaledThickness.Right + this._resizeBorderWidth;
                height = scaledThickness.Top + this._resizeBorderWidth;
            }

            if (this._actionButtonBorderRight != null)
            {
                this._actionButtonBorderRight.Width = width;
            }

            if (this._actionButtonBorderTop != null)
            {
                this._actionButtonBorderTop.Height = height;
            }
        }

        private void SetWindowBorderBrush(Brush brush)
        {
            this._container?.SetValue(Border.BorderBrushProperty, brush);
        }

        private void SetTitleBarBackground(Brush brush)
        {
            this._titleBarPanel?.SetValue(Panel.BackgroundProperty, brush);
        }

        private T FindTemplatePart<T>(string name) where T : FrameworkElement
        {
            return this.Template?.FindName(name, this) as T;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._container = this.FindTemplatePart<Border>("PART_Container");
            this._actionButtonBorderTop = this.FindTemplatePart<FrameworkElement>("PART_ActionButtonBorderTop");
            this._actionButtonBorderRight = this.FindTemplatePart<FrameworkElement>("PART_ActionButtonBorderRight");

            this._minimizeButton = this.FindTemplatePart<Button>("PART_MinimizeButton");
            this._maximizeButton = this.FindTemplatePart<Button>("PART_MaximizeButton");
            this._restoreButton = this.FindTemplatePart<Button>("PART_RestoreButton");
            this._closeButton = this.FindTemplatePart<Button>("PART_CloseButton");

            this._titleBarPanel = this.FindTemplatePart<Panel>("PART_TitleBarPanel");

            this.UpdateWindowDecoration();
            this.UpdateTitleBarColorization();
        }

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            base.OnDpiChanged(oldDpi, newDpi);

            this.SetScale(newDpi);

            this.UpdateWindowDecoration();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            this.UpdateWindowDecoration();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            this.UpdateTitleBarColorization();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);

            this.UpdateTitleBarColorization();
        }

        private void UpdateCaptionButtonState()
        {
            if (this.WindowStyle == WindowStyle.ToolWindow || this.ResizeMode == ResizeMode.NoResize)
            {
                this.SetActionButtonVisibility(false, false, false, true);
            }
            else
            {
                switch (this.WindowState)
                {
                    case System.Windows.WindowState.Minimized:
                        this.SetActionButtonVisibility(false, false, true, true);
                        break;

                    case System.Windows.WindowState.Maximized:
                        this.SetActionButtonVisibility(true, false, true, true);
                        break;

                    case System.Windows.WindowState.Normal:
                        this.SetActionButtonVisibility(true, true, false, true);
                        break;
                }
            }
        }

        private void SetActionButtonVisibility(bool minimize, bool maximize, bool restore, bool close)
        {
            this._minimizeButton?.SetValue(VisibilityProperty, BoolToVisibility(minimize));
            this._maximizeButton?.SetValue(VisibilityProperty, BoolToVisibility(maximize));
            this._restoreButton?.SetValue(VisibilityProperty, BoolToVisibility(restore));
            this._closeButton?.SetValue(VisibilityProperty, BoolToVisibility(close));
        }

        private void UpdateWindowDecoration()
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                this.SetWindowBorderThickness(new Thickness(0));
                this.SetPadding(this._maximizedBorderMargin);

                double captionHeightOffset = Rescale(this._borderPadWidth, this._scale);
                this.SetCaptionHeight(captionHeightOffset + this.CaptionHeight);
            }
            else
            {
                this.SetWindowBorderThickness(this.BorderThickness);
                this.SetPadding(new Thickness(0));

                double captionHeightOffset = Rescale(this.BorderThickness.Top, this._scale) - this._borderPadWidth;
                this.SetCaptionHeight(captionHeightOffset + this.CaptionHeight);
            }

            this.UpdateCaptionButtonState();
        }

        private void UpdateTitleBarColorization()
        {
            if (this.IsActive)
            {
                this.SetTitleBarBackground(this.TitleBarActiveBackground);
                this.SetTitleBarForeground(this.TitleBarActiveForeground);
                this.SetWindowBorderBrush(this.ActiveBorderBrush);
            }
            else
            {
                this.SetTitleBarBackground(this.TitleBarInactiveBackground);
                this.SetTitleBarForeground(this.TitleBarInactiveForeground);
                this.SetWindowBorderBrush(this.InactiveBorderBrush);
            }
        }

        private static Visibility BoolToVisibility(bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        private static double Rescale(double value, double scale) => value / scale;

        private static Thickness Rescale(in Thickness thickness, double scale) => new Thickness
        {
            Top = thickness.Top / scale,
            Left = thickness.Left / scale,
            Right = thickness.Right / scale,
            Bottom = thickness.Bottom / scale,
        };

        #region UI Properties

        [Bindable(true)]
        public object TitleBarContent
        {
            get => this.GetValue(TitleBarContentProperty);
            set => this.SetValue(TitleBarContentProperty, value);
        }

        public static readonly DependencyProperty TitleBarContentProperty =
            DependencyProperty.Register("TitleBarContent", typeof(object), typeof(CustomWindow), new PropertyMetadata(0));


        public double CaptionHeight
        {
            get => (double)this.GetValue(CaptionHeightProperty);
            set => this.SetValue(CaptionHeightProperty, value);
        }

        public static readonly DependencyProperty CaptionHeightProperty =
            DependencyProperty.Register("CaptionHeight", typeof(double), typeof(CustomWindow),
                new PropertyMetadata(30.0d, OnCaptionHeightPropertyChanged));

        private static void OnCaptionHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomWindow window)
            {
                window.UpdateWindowDecoration();
            }
        }

        public Brush TitleBarActiveForeground
        {
            get => (Brush)this.GetValue(TitleBarActiveForegroundProperty);
            set => this.SetValue(TitleBarActiveForegroundProperty, value);
        }

        public static readonly DependencyProperty TitleBarActiveForegroundProperty =
            DependencyProperty.Register("TitleBarActiveForeground", typeof(Brush), typeof(CustomWindow),
                new FrameworkPropertyMetadata(SystemColors.ActiveCaptionTextBrush, OnDecorationColorPropertyChanged));

        public Brush TitleBarInactiveForeground
        {
            get => (Brush)this.GetValue(TitleBarInactiveForegroundProperty);
            set => this.SetValue(TitleBarInactiveForegroundProperty, value);
        }

        public static readonly DependencyProperty TitleBarInactiveForegroundProperty =
            DependencyProperty.Register("TitleBarInactiveForeground", typeof(Brush), typeof(CustomWindow),
                new FrameworkPropertyMetadata(SystemColors.InactiveCaptionTextBrush, OnDecorationColorPropertyChanged));

        public Brush TitleBarActiveBackground
        {
            get => (Brush)this.GetValue(TitleBarActiveBackgroundProperty);
            set => this.SetValue(TitleBarActiveBackgroundProperty, value);
        }

        public static readonly DependencyProperty TitleBarActiveBackgroundProperty =
            DependencyProperty.Register("TitleBarActiveBackground", typeof(Brush), typeof(CustomWindow),
                new FrameworkPropertyMetadata(SystemColors.ActiveCaptionBrush, OnDecorationColorPropertyChanged));

        public Brush TitleBarInactiveBackground
        {
            get => (Brush)this.GetValue(TitleBarInactiveBackgroundProperty);
            set => this.SetValue(TitleBarInactiveBackgroundProperty, value);
        }

        public static readonly DependencyProperty TitleBarInactiveBackgroundProperty =
            DependencyProperty.Register("TitleBarInactiveBackground", typeof(Brush), typeof(CustomWindow),
                new FrameworkPropertyMetadata(SystemColors.InactiveCaptionBrush, OnDecorationColorPropertyChanged));

        public Brush ActiveBorderBrush
        {
            get => (Brush)this.GetValue(ActiveBorderBrushProperty);
            set => SetValue(ActiveBorderBrushProperty, value);
        }

        public static readonly DependencyProperty ActiveBorderBrushProperty =
            DependencyProperty.Register("ActiveBorderBrush", typeof(Brush), typeof(CustomWindow),
                new FrameworkPropertyMetadata(SystemColors.ActiveBorderBrush, OnDecorationColorPropertyChanged));

        public Brush InactiveBorderBrush
        {
            get => (Brush)this.GetValue(InactiveBorderBrushProperty);
            set => this.SetValue(InactiveBorderBrushProperty, value);
        }

        public static readonly DependencyProperty InactiveBorderBrushProperty =
            DependencyProperty.Register("InactiveBorderBrush", typeof(Brush), typeof(CustomWindow),
                new FrameworkPropertyMetadata(SystemColors.InactiveBorderBrush, OnDecorationColorPropertyChanged));

        private static void OnDecorationColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomWindow window)
            {
                window.UpdateTitleBarColorization();
            }
        }

        #endregion

        #region Properties

        public static bool GetIsHitTestVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHitTestVisibleInCaptionProperty);
        }

        public static void SetIsHitTestVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHitTestVisibleInCaptionProperty, value);
        }

        public static readonly DependencyProperty IsHitTestVisibleInCaptionProperty =
            DependencyProperty.RegisterAttached("IsHitTestVisible", typeof(bool), typeof(CustomWindow),
                new PropertyMetadata(false, IsHitTestVisibleInCaptionPropertyChanged));

        private static void IsHitTestVisibleInCaptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IInputElement element)
            {
                WindowChrome.SetIsHitTestVisibleInChrome(element, (bool)e.NewValue);
            }
        }

        #endregion
    }
}
