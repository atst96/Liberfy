using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Liberfy
{
    static class WindowExtensions
    {
        public static WindowStatus GetStatus(DependencyObject obj)
        {
            return (WindowStatus)obj.GetValue(StatusProperty);
        }

        public static void SetStatus(DependencyObject obj, WindowStatus value)
        {
            obj.SetValue(StatusProperty, value);
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.RegisterAttached("Status",
                typeof(WindowStatus), typeof(WindowExtensions),
                new PropertyMetadata(null));
    }

    class WindowStatusExtension : MarkupExtension
    {
        public WindowStatus Status { get; set; }

        public bool LoadHeight { get; set; } = true;
        public bool LoadWidth { get; set; } = true;
        public bool LoadPosition { get; set; } = true;
        public bool LoadState { get; set; } = true;

        public bool SaveHeight { get; set; } = true;
        public bool SaveWidth { get; set; } = true;
        public bool SavePosition { get; set; } = true;
        public bool SaveState { get; set; } = true;

        private Window _view;
        private IProvideValueTarget _provider;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            this._provider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            this._view = _provider.TargetObject as Window;

            if (this._view == null)
                throw new NotSupportedException();

            if (this.Status == null)
                throw new NullReferenceException(nameof(Status));

            this._view.Loaded += _view_Loaded;
            this._view.Closed += _view_Closed;
            this._view.StateChanged += _view_StateChanged;
            this._view.SizeChanged += _view_SizeChanged;
            this._view.LocationChanged += _view_LocationChanged;

            return Status;
        }

        private void LoadStatus()
        {
            if(this.LoadWidth && this.Status.Width.HasValue)
                this._view.Width = this.Status.Width.Value;

            if (this.LoadHeight && this.Status.Height.HasValue)
                this._view.Height = this.Status.Height.Value;

            if (this.LoadPosition)
            {
                if (this.Status.Top.HasValue)
                    this._view.Top = this.Status.Top.Value;

                if (this.Status.Left.HasValue)
                    this._view.Left = this.Status.Left.Value;
            }

            if(this.LoadState && this.Status.State.HasValue)
                this._view.WindowState = this.Status.State.Value;
        }

        private bool IsNormalState => this._view.WindowState == WindowState.Normal;

        private void _view_LocationChanged(object sender, EventArgs e)
        {
            if (this.SavePosition && this.IsNormalState)
            {
                this.Status.Left = this._view.Left;
                this.Status.Top = this._view.Top;
            }
        }

        private void _view_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.IsNormalState)
            {
                if (this.SaveWidth)
                    this.Status.Width = this._view.ActualWidth;

                if (this.SaveHeight)
                    this.Status.Height = this._view.ActualHeight;
            }
        }

        private void _view_StateChanged(object sender, EventArgs e)
        {
            if(this.SaveState)
            {
                this.Status.State = this._view.WindowState;
            }
        }

        private void _view_Closed(object sender, EventArgs e)
        {
            this._view.Loaded -= this._view_Loaded;
            this._view.Closed -= this._view_Closed;
            this._view.StateChanged -= this._view_StateChanged;
            this._view.SizeChanged -= this._view_SizeChanged;
            this._view.LocationChanged -= this._view_LocationChanged;
        }

        private void _view_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadStatus();
        }
    }
}
