using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Liberfy.Behaviors
{
    public class DragDropBehavior : System.Windows.Interactivity.Behavior<FrameworkElement>, ICommandSource
    {
        private DragDropHelper _dragDropHelper;

        object ICommandSource.CommandParameter => throw new NotImplementedException();

        IInputElement ICommandSource.CommandTarget => throw new NotImplementedException();

        protected override void OnAttached()
        {
            this.AssociatedObject.PreviewDragEnter += OnDragEnter;
            this.AssociatedObject.PreviewDragOver += OnDragOver;
            this.AssociatedObject.PreviewDragLeave += OnDragLeave;
            this.AssociatedObject.PreviewDrop += OnDrop;

            this._dragDropHelper = new DragDropHelper();

            if (this.AssociatedObject.IsInitialized)
            {
                this._dragDropHelper.SetHandle(GetHandle(this.AssociatedObject));
            }
            else
            {
                this.AssociatedObject.Loaded += this.OnTargetLoaded;
            }

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.Loaded -= this.OnTargetLoaded;
            this.AssociatedObject.PreviewDragEnter -= this.OnDragEnter;
            this.AssociatedObject.PreviewDragOver -= this.OnDragOver;
            this.AssociatedObject.PreviewDragLeave -= this.OnDragLeave;
            this.AssociatedObject.PreviewDrop -= this.OnDrop;

            this._dragDropHelper.Dispose();
            this._dragDropHelper = null;

            base.OnDetaching();
        }

        private void OnTargetLoaded(object sender, EventArgs e)
        {
            this._dragDropHelper.SetHandle(GetHandle(this.AssociatedObject));
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

            if (this.Command?.CanExecute(e.Data) ?? false)
            {
                effect = e.AllowedEffects & AllowEffects;
            }
            else
            {
                effect = DragDropEffects.None;
            }

            if (this.SetDescription)
            {
                if (this.UseDescriptionIcon)
                {
                    DragDropHelper.SetDescription(
                        e.Data,
                        this.DescriptionIcon,
                        this.DescriptionMessage,
                        this.DescriptionInsertion);
                }
                else
                {
                    DragDropHelper.SetDescription(
                        e.Data, effect,
                        this.DescriptionMessage,
                        this.DescriptionInsertion);
                }
            }

            this._dragDropHelper.DragEnter(e.Data, e.GetPosition(this.AssociatedObject), effect);

            e.Handled = true;
            e.Effects = effect;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            if (IsTextBoxTextDrag(e))
                return;

            DragDropEffects effect;
            if (this.Command?.CanExecute(e.Data) ?? false)
            {
                effect = e.AllowedEffects & AllowEffects;
            }
            else
            {
                effect = DragDropEffects.None;
            }

            e.Effects = effect;

            this._dragDropHelper.DragOver(e.GetPosition(this.AssociatedObject), effect);
            e.Handled = true;
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            if (IsTextBoxTextDrag(e))
                return;

            this._dragDropHelper.DragLeave();
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (IsTextBoxTextDrag(e))
                return;

            DragDropEffects effect;
            bool canDrop = this.Command?.CanExecute(e.Data) ?? false;

            if (canDrop)
            {
                effect = e.AllowedEffects & this.AllowEffects;
            }
            else
            {
                effect = DragDropEffects.None;
            }

            e.Effects = effect;

            this._dragDropHelper.Drop(e.Data, e.GetPosition(this.AssociatedObject), effect);

            if (canDrop)
            {
                this.Command?.Execute(e.Data);
            }

            e.Handled = true;
        }

        #endregion

        private static bool IsTextBoxTextDrag(DragEventArgs eventArgs)
        {
            var formats = eventArgs.Data.GetFormats();

            return formats.Length == 3
                && formats.Contains("Text")
                && formats.Contains("UnicodeText")
                && formats.Contains("System.String");
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

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command",
                typeof(ICommand), typeof(DragDropBehavior),
                new PropertyMetadata(null));


        public static readonly DependencyProperty AllowEffectsProperty =
            DependencyProperty.Register("AllowEffects",
                typeof(DragDropEffects), typeof(DragDropBehavior));

        public static readonly DependencyProperty DescriptionIconProperty =
            DependencyProperty.Register("DescriptionIcon",
                typeof(DropImageType), typeof(DragDropBehavior),
                new PropertyMetadata(DropImageType.None));

        public static readonly DependencyProperty DescriptionMessageProperty =
            DependencyProperty.Register("DescriptionMessage",
                typeof(string), typeof(DragDropBehavior),
                new PropertyMetadata(null));

        public static readonly DependencyProperty DescriptionInsertionProperty =
            DependencyProperty.Register("DescriptionInsertion",
                typeof(string), typeof(DragDropBehavior),
                new PropertyMetadata(null));
    }
}
