using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Liberfy
{
    /// <summary>
    /// ウィザード形式のパネル
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(WizardPage))]
    internal class WizardPanel : TabControl
    {
        /// <summary>
        /// [前へ]ボタンのコマンドのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register(nameof(PreviousCommand), typeof(ICommand), typeof(WizardPanel), new PropertyMetadata(null));

        /// <summary>
        /// [前へ]ボタンを表示するかどうかのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsShowPreviousButtonProperty =
            DependencyProperty.Register(nameof(IsShowPreviousButton), typeof(bool), typeof(WizardPanel), new PropertyMetadata(true));

        /// <summary>
        /// [前へ]ボタンを強調表示するかどうかのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsEmphasisPreviousButtonProperty =
            DependencyProperty.Register(nameof(IsEmphasisPreviousButton), typeof(bool), typeof(WizardPanel), new PropertyMetadata(false));

        /// <summary>
        /// [前へ]ボタンのコマンドを取得または設定する。
        /// </summary>
        public ICommand PreviousCommand
        {
            get => (ICommand)this.GetValue(PreviousCommandProperty);
            set => this.SetValue(PreviousCommandProperty, value);
        }

        /// <summary>
        /// [前へ]ボタンを表示するかどうかを取得または設定する。
        /// </summary>
        public bool IsShowPreviousButton
        {
            get => (bool)this.GetValue(IsShowPreviousButtonProperty);
            set => this.SetValue(IsShowPreviousButtonProperty, value);
        }

        /// <summary>
        /// [前へ]ボタンを強調表示するかどうかを取得または設定する
        /// </summary>
        public bool IsEmphasisPreviousButton
        {
            get => (bool)this.GetValue(IsEmphasisPreviousButtonProperty);
            set => this.SetValue(IsEmphasisPreviousButtonProperty, value);
        }

        /// <summary>
        /// [次へ]ボタンのコマンドのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register(nameof(NextCommand), typeof(ICommand), typeof(WizardPanel), new PropertyMetadata(null));

        /// <summary>
        /// [次へ]ボタンを表示するかどうかのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsShowNextButtonProperty =
            DependencyProperty.Register(nameof(IsShowNextButton), typeof(bool), typeof(WizardPanel), new PropertyMetadata(true));

        /// <summary>
        /// [次へ]ボタンを強調表示にするかどうかのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsEmphasisNextButtonProperty =
            DependencyProperty.Register(nameof(IsEmphasisNextButton), typeof(bool), typeof(WizardPanel), new PropertyMetadata(false));

        /// <summary>
        /// [次へ]ボタンのコマンドを取得または設定する。
        /// </summary>
        public ICommand NextCommand
        {
            get => (ICommand)this.GetValue(NextCommandProperty);
            set => this.SetValue(NextCommandProperty, value);
        }

        /// <summary>
        /// [次へ]ボタンを表示するかどうかを取得または設定する。
        /// </summary>
        public bool IsShowNextButton
        {
            get => (bool)this.GetValue(IsShowNextButtonProperty);
            set => this.SetValue(IsShowNextButtonProperty, value);
        }

        /// <summary>
        /// [次へ]ボタンを強調表示にするかどうかを取得または設定する。
        /// </summary>
        public bool IsEmphasisNextButton
        {
            get => (bool)this.GetValue(IsEmphasisNextButtonProperty);
            set => this.SetValue(IsEmphasisNextButtonProperty, value);
        }

        /// <summary>
        /// [キャンセル]ボタンのコマンドのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(WizardPanel), new PropertyMetadata(null));

        /// <summary>
        /// [キャンセル]ボタンを表示するかどうかのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsShowCancelButtonProperty =
            DependencyProperty.Register(nameof(IsShowCancelButton), typeof(bool), typeof(WizardPanel), new PropertyMetadata(true));

        /// <summary>
        /// [キャンセル]ボタンを強調表示するかどうかのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsEmphasisCancelButtonProperty =
            DependencyProperty.Register(nameof(IsEmphasisCancelButton), typeof(bool), typeof(WizardPanel), new PropertyMetadata(false));

        /// <summary>
        /// [キャンセル]ボタンのコマンドを取得または設定する。
        /// </summary>
        public ICommand CancelCommand
        {
            get => (ICommand)this.GetValue(CancelCommandProperty);
            set => this.SetValue(CancelCommandProperty, value);
        }

        /// <summary>
        /// [キャンセル]ボタンを表示するかどうかを取得または設定する。
        /// </summary>
        public bool IsShowCancelButton
        {
            get => (bool)this.GetValue(IsShowCancelButtonProperty);
            set => this.SetValue(IsShowCancelButtonProperty, value);
        }

        /// <summary>
        /// [キャンセル]ボタンを強調表示するかどうかを取得または設定する。
        /// </summary>
        public bool IsEmphasisCancelButton
        {
            get => (bool)this.GetValue(IsEmphasisCancelButtonProperty);
            set => this.SetValue(IsEmphasisCancelButtonProperty, value);
        }

        /// <summary>
        /// ビジー状態かどうかのDependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(WizardPanel), new PropertyMetadata(false));

        /// <summary>
        /// ビジー状態かどうかを取得または設定する。
        /// </summary>
        public bool IsBusy
        {
            get => (bool)this.GetValue(IsBusyProperty);
            set => this.SetValue(IsBusyProperty, value);
        }

        /// <summary>
        /// ボタンパネルの背景色のDependencyProperty
        /// </summary>
        public static readonly DependencyProperty ButtonPanelBackgroundProperty =
            DependencyProperty.Register(nameof(ButtonPanelBackground), typeof(Brush), typeof(WizardPanel), new PropertyMetadata(null));

        /// <summary>
        /// ボタンパネルの背景色を取得または設定する。
        /// </summary>
        public Brush ButtonPanelBackground
        {
            get => (Brush)this.GetValue(ButtonPanelBackgroundProperty);
            set => this.SetValue(ButtonPanelBackgroundProperty, value);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new WizardPage();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is WizardPage;
        }
    }
}
