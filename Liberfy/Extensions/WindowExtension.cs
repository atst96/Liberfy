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
			_provider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			_view = _provider.TargetObject as Window;

			if (_view == null)
			{
				throw new NotSupportedException();
			}

			if (Status == null)
			{
				throw new NullReferenceException(nameof(Status));
			}

			_view.Loaded += _view_Loaded;
			_view.Closed += _view_Closed;
			_view.StateChanged += _view_StateChanged;
			_view.SizeChanged += _view_SizeChanged;
			_view.LocationChanged += _view_LocationChanged;

			return Status;
		}

		private void LoadStatus()
		{
			if(LoadWidth && Status.Width.HasValue)
			{
				_view.Width = Status.Width.Value;
			}

			if(LoadHeight && Status.Height.HasValue)
			{
				_view.Height = Status.Height.Value;
			}

			if(LoadPosition)
			{
				if (Status.Top.HasValue)
					_view.Top = Status.Top.Value;

				if (Status.Left.HasValue)
					_view.Left = Status.Left.Value;
			}

			if(LoadState && Status.State.HasValue)
			{
				_view.WindowState = Status.State.Value;
			}
		}

		private bool IsNormalState => _view.WindowState == WindowState.Normal;

		private void _view_LocationChanged(object sender, EventArgs e)
		{
			if (SavePosition && IsNormalState)
			{
				Status.Left = _view.Left;
				Status.Top = _view.Top;
			}
		}

		private void _view_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (IsNormalState)
			{
				if (SaveWidth)
					Status.Width = _view.ActualWidth;

				if (SaveHeight)
					Status.Height = _view.ActualHeight;
			}
		}

		private void _view_StateChanged(object sender, EventArgs e)
		{
			if(SaveState)
			{
				Status.State = _view.WindowState;
			}
		}

		private void _view_Closed(object sender, EventArgs e)
		{
			_view.Loaded -= _view_Loaded;
			_view.Closed -= _view_Closed;
			_view.StateChanged -= _view_StateChanged;
			_view.SizeChanged -= _view_SizeChanged;
			_view.LocationChanged -= _view_LocationChanged;
		}

		private void _view_Loaded(object sender, RoutedEventArgs e)
		{
			LoadStatus();
		}
	}
}
