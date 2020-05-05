using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Liberfy.Behaviors
{
    /// <summary>
    /// ドラッグ＆ドロップ操作ようビヘイビア
    /// </summary>
    public class DragDropBehavior : Microsoft.Xaml.Behaviors.Behavior<FrameworkElement>, ICommandSource
    {
        /// <summary>
        /// DragDropHandler
        /// </summary>
        private DragDropHelper _dragDropHelper;

        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        private IntPtr _handle;

        object ICommandSource.CommandParameter => throw new NotImplementedException();

        IInputElement ICommandSource.CommandTarget => throw new NotImplementedException();

        /// <summary>
        /// AssociatedObjectアタッチ時
        /// </summary>
        protected override void OnAttached()
        {
            var target = this.AssociatedObject;
            target.PreviewDragEnter += this.OnDragEnter;
            target.PreviewDragOver += this.OnDragOver;
            target.PreviewDragLeave += this.OnDragLeave;
            target.PreviewDrop += this.OnDrop;

            this._dragDropHelper = new DragDropHelper();

            if (target.IsInitialized)
            {
                this.OnInitialized();
            }
            else
            {
                target.Loaded += this.OnAssociatedObjectLoaded;
            }

            base.OnAttached();
        }

        /// <summary>
        /// AssociatedObjectデタッチ時
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            var target = this.AssociatedObject;
            target.Loaded -= this.OnAssociatedObjectLoaded;
            target.PreviewDragEnter -= this.OnDragEnter;
            target.PreviewDragOver -= this.OnDragOver;
            target.PreviewDragLeave -= this.OnDragLeave;
            target.PreviewDrop -= this.OnDrop;

            this._dragDropHelper.Dispose();
            this._dragDropHelper = null;
        }

        /// <summary>
        /// AssociatedObject初期化時
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">イベント引数</param>
        private void OnAssociatedObjectLoaded(object sender, EventArgs e)
        {
            this.OnInitialized();
        }

        /// <summary>
        /// 初期化時
        /// </summary>
        private void OnInitialized()
        {
            var target = this.AssociatedObject;

            this._handle = GetWindowHandle(target);
            this._dragDropHelper.SetHandle(this._handle);
        }

        /// <summary>
        /// FrameworkElementからウィンドウハンドルを取得する。
        /// </summary>
        /// <param name="element"></param>
        /// <returns>ウィンドウハンドル</returns>
        private static IntPtr GetWindowHandle(FrameworkElement element)
        {
            return ((HwndSource)PresentationSource.FromVisual(element)).Handle;
        }

        /// <summary>
        /// ドラッグ＆ドロップ操作の開始
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">ドラッグイベント情報</param>
        private void OnDragEnter(object sender, DragEventArgs e)
        {
            if (IsDropFromTextBox(e))
            {
                return;
            }

            bool canExecute = this.Command?.CanExecute(e.Data) ?? false;

            var effect = canExecute ? (e.AllowedEffects & this.AllowEffects) : DragDropEffects.None;

            if (this.SetDescription)
            {
                if (this.UseDescriptionIcon)
                {
                    DragDropHelper.SetDescription(e.Data, this.DescriptionIcon, this.DescriptionMessage, this.DescriptionInsertion);
                }
                else
                {
                    DragDropHelper.SetDescription(e.Data, effect, this.DescriptionMessage, this.DescriptionInsertion);
                }
            }

            var position = e.GetPosition(this.AssociatedObject);
            this._dragDropHelper.DragEnter(e.Data, position, effect);

            e.Effects = effect;
            e.Handled = true;
        }

        /// <summary>
        /// ドラッグ＆ドロップ操作のマウス移動時
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">ドラッグイベント情報</param>
        private void OnDragOver(object sender, DragEventArgs e)
        {
            if (IsDropFromTextBox(e))
            {
                return;
            }

            DragDropEffects effect;
            if (this.Command?.CanExecute(e.Data) ?? false)
            {
                effect = e.AllowedEffects & AllowEffects;
            }
            else
            {
                effect = DragDropEffects.None;
            }

            var position = e.GetPosition(this.AssociatedObject);
            this._dragDropHelper.DragOver(position, effect);

            e.Effects = effect;
            e.Handled = true;
        }

        /// <summary>
        /// ドラッグ＆ドロップ操作の終了
        /// </summary>
        /// <param name="sender">イベント発行元</param>
        /// <param name="e">ドラッグイベント情報</param>
        private void OnDragLeave(object sender, DragEventArgs e)
        {
            if (IsDropFromTextBox(e))
            {
                return;
            }

            // DragDropHandlerにDragLeaveを通知する
            this._dragDropHelper.DragLeave();

            e.Handled = true;
        }

        /// <summary>
        /// ドラッグ＆ドロップ操作のドロップ時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDrop(object sender, DragEventArgs e)
        {
            if (IsDropFromTextBox(e))
            {
                return;
            }

            // 実行判定
            bool canExecute = this.Command?.CanExecute(e.Data) ?? false;

            var effect = canExecute
                ? (e.AllowedEffects & this.AllowEffects)
                : DragDropEffects.None;

            // DragDropHandlerにDropを通知する
            var position = e.GetPosition(this.AssociatedObject);
            this._dragDropHelper.Drop(e.Data, position, effect);

            // コマンド実行
            if (canExecute)
            {
                this.Command.Execute(e.Data);
            }

            e.Effects = effect;
            e.Handled = true;
        }

        /// <summary>
        /// テキストボックスからのドラッグ＆ドロップ操作かどうかを判定する。
        /// </summary>
        /// <param name="eventArgs">ドラッグイベント情報</param>
        /// <returns>テキストボックスからのドラッグ＆ドロップ操作かどうか</returns>
        private static bool IsDropFromTextBox(DragEventArgs eventArgs)
        {
            var formats = eventArgs.Data.GetFormats();

            return formats.Length == 3
                && formats.Contains("Text")
                && formats.Contains("UnicodeText")
                && formats.Contains("System.String");
        }

        /// <summary>
        /// コマンドのプロパティ
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(DragDropBehavior), new PropertyMetadata(null));

        /// <summary>
        /// コマンドを取得または設定する。
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)this.GetValue(CommandProperty);
            set => this.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// 実行可能なドラッグ＆ドロップ操作のプロパティ
        /// </summary>
        public static readonly DependencyProperty AllowEffectsProperty =
            DependencyProperty.Register(nameof(AllowEffects), typeof(DragDropEffects), typeof(DragDropBehavior));

        /// <summary>
        /// 実行可能なドラッグ＆ドロップ操作を取得または設定する。
        /// </summary>
        public DragDropEffects AllowEffects
        {
            get => (DragDropEffects)this.GetValue(AllowEffectsProperty);
            set => this.SetValue(AllowEffectsProperty, value);
        }

        /// <summary>
        /// ドラッグ表示時のアイコンを使用するかどうかを取得または設定する。
        /// </summary>
        public bool UseDescriptionIcon { get; set; }

        /// <summary>
        /// ドラッグ操作時アイコンのプロパティ
        /// </summary>
        public static readonly DependencyProperty DescriptionIconProperty =
            DependencyProperty.Register(nameof(DescriptionIcon), typeof(DropImageType), typeof(DragDropBehavior), new PropertyMetadata(DropImageType.None));

        /// <summary>
        /// ドラッグ操作時に表示するアイコンを取得または設定する。
        /// </summary>
        public DropImageType DescriptionIcon
        {
            get => (DropImageType)this.GetValue(DescriptionIconProperty);
            set => this.SetValue(DescriptionIconProperty, value);
        }

        /// <summary>
        /// ドラッグ操作時にラベルを表示するかどうかを取得または設定する。
        /// </summary>
        public bool SetDescription { get; set; }

        /// <summary>
        /// ドラッグ操作時ラベルのプロパティ
        /// </summary>
        public static readonly DependencyProperty DescriptionMessageProperty =
            DependencyProperty.Register(nameof(DescriptionMessage), typeof(string), typeof(DragDropBehavior), new PropertyMetadata(null));

        /// <summary>
        /// ドラッグ操作時のラベルを取得または設定する。
        /// </summary>
        public string DescriptionMessage
        {
            get => (string)this.GetValue(DescriptionMessageProperty);
            set => this.SetValue(DescriptionMessageProperty, value);
        }

        /// <summary>
        /// ドラッグ操作時ラベル補足のプロパティ
        /// </summary>
        public static readonly DependencyProperty DescriptionInsertionProperty =
            DependencyProperty.Register(nameof(DescriptionInsertion), typeof(string), typeof(DragDropBehavior), new PropertyMetadata(null));

        /// <summary>
        /// ドラッグ操作時のラベルの補足
        /// </summary>
        public string DescriptionInsertion
        {
            get => (string)this.GetValue(DescriptionInsertionProperty);
            set => this.SetValue(DescriptionInsertionProperty, value);
        }
    }
}
