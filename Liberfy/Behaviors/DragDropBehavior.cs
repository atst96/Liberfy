using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Interop;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Liberfy.Behaviors
{
	class DragDropBehavior : Behavior<FrameworkElement>, ICommandSource
	{
		private DragDropHelper _dragDropHelper;

		object ICommandSource.CommandParameter => throw new NotImplementedException();

		IInputElement ICommandSource.CommandTarget => throw new NotImplementedException();

		protected override void OnAttached()
		{
			AssociatedObject.PreviewDragEnter += OnDragEnter;
			AssociatedObject.PreviewDragOver += OnDragOver;
			AssociatedObject.PreviewDragLeave += OnDragLeave;
			AssociatedObject.PreviewDrop += OnDrop;

			_dragDropHelper = new DragDropHelper();

			if (AssociatedObject.IsInitialized)
			{
				_dragDropHelper.SetHandle(GetHandle(AssociatedObject));
			}
			else
			{
				AssociatedObject.Loaded += OnTargetLoaded;
			}

			base.OnAttached();
		}

		protected override void OnDetaching()
		{
			AssociatedObject.Loaded -= OnTargetLoaded;
			AssociatedObject.PreviewDragEnter -= OnDragEnter;
			AssociatedObject.PreviewDragOver -= OnDragOver;
			AssociatedObject.PreviewDragLeave -= OnDragLeave;
			AssociatedObject.PreviewDrop -= OnDrop;

			_dragDropHelper.Dispose();
			_dragDropHelper = null;

			base.OnDetaching();
		}

		private void OnTargetLoaded(object sender, EventArgs e)
		{
			_dragDropHelper.SetHandle(GetHandle(AssociatedObject));
		}

		private static IntPtr GetHandle(FrameworkElement element)
		{
			return ((HwndSource)PresentationSource.FromVisual(element)).Handle;
		}

		#region Drag and drop methods

		private void OnDragEnter(object sender, DragEventArgs e)
		{
			if (IsTextBoxTextDrag(e))
				return;

			DragDropEffects effect;

			if (Command?.CanExecute(e.Data) ?? false)
			{
				effect = e.AllowedEffects & AllowEffects;
			}
			else
			{
				effect = DragDropEffects.None;
			}

			if (SetDescription)
			{
				if (UseDescriptionIcon)
				{
					DragDropHelper.SetDescription(
						e.Data,
						DescriptionIcon,
						DescriptionMessage,
						DescriptionInsertion);
				}
				else
				{
					DragDropHelper.SetDescription(
						e.Data, effect,
						DescriptionMessage,
						DescriptionInsertion);
				}
			}

			_dragDropHelper.DragEnter(e.Data, e.GetPosition(AssociatedObject), effect);

			e.Handled = true;
			e.Effects = effect;
		}

		private void OnDragOver(object sender, DragEventArgs e)
		{
			if (IsTextBoxTextDrag(e))
				return;

			DragDropEffects effect;
			if (Command?.CanExecute(e.Data) ?? false)
			{
				effect = e.AllowedEffects & AllowEffects;
			}
			else
			{
				effect = DragDropEffects.None;
			}

			e.Effects = effect;

			_dragDropHelper.DragOver(e.GetPosition(AssociatedObject), effect);
			e.Handled = true;
		}

		private void OnDragLeave(object sender, DragEventArgs e)
		{
			if (IsTextBoxTextDrag(e))
				return;

			_dragDropHelper.DragLeave();
			e.Handled = true;
		}

		private void OnDrop(object sender, DragEventArgs e)
		{
			if (IsTextBoxTextDrag(e))
				return;

			DragDropEffects effect;
			bool canDrop = Command?.CanExecute(e.Data) ?? false;

			if (canDrop)
			{
				effect = e.AllowedEffects & AllowEffects;
			}
			else
			{
				effect = DragDropEffects.None;
			}

			e.Effects = effect;

			_dragDropHelper.Drop(e.Data, e.GetPosition(AssociatedObject), effect);

			if (canDrop)
			{
				Command?.Execute(e.Data);
			}

			e.Handled = true;
		}

		#endregion

		private static bool IsTextBoxTextDrag(DragEventArgs eventArgs)
		{
			var formats = eventArgs.Data.GetFormats();

			return formats.Length != 3
				|| !formats.Contains("Text")
				|| !formats.Contains("UnicodeText")
				|| !formats.Contains("System.String");
		}

		public ICommand Command
		{
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		public DragDropEffects AllowEffects
		{
			get => (DragDropEffects)GetValue(AllowEffectsProperty);
			set => SetValue(AllowEffectsProperty, value);
		}

		public bool UseDescriptionIcon { get; set; }

		public bool SetDescription { get; set; }

		public DropImageType DescriptionIcon
		{
			get => (DropImageType)GetValue(DescriptionIconProperty);
			set => SetValue(DescriptionIconProperty, value);
		}

		public string DescriptionMessage
		{
			get => (string)GetValue(DescriptionMessageProperty);
			set => SetValue(DescriptionMessageProperty, value);
		}

		public string DescriptionInsertion
		{
			get => (string)GetValue(DescriptionInsertionProperty);
			set => SetValue(DescriptionInsertionProperty, value);
		}

		public static readonly DependencyProperty CommandProperty
			= DependencyProperty.Register("Command",
				typeof(ICommand), typeof(DragDropBehavior), new PropertyMetadata(null));


		public static readonly DependencyProperty AllowEffectsProperty
			= DependencyProperty.Register("AllowEffects",
				typeof(DragDropEffects), typeof(DragDropBehavior));

		public static readonly DependencyProperty DescriptionIconProperty
			= DependencyProperty.Register("DescriptionIcon",
				typeof(DropImageType), typeof(DragDropBehavior), new PropertyMetadata(DropImageType.None));

		public static readonly DependencyProperty DescriptionMessageProperty
			= DependencyProperty.Register("DescriptionMessage",
				typeof(string), typeof(DragDropBehavior), new PropertyMetadata(null));

		public static readonly DependencyProperty DescriptionInsertionProperty
			= DependencyProperty.Register("DescriptionInsertion",
				typeof(string), typeof(DragDropBehavior), new PropertyMetadata(null));
	}
}
