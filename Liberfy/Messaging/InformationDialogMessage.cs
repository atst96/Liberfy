using System.Windows;
using System.Windows.Forms;
using Livet.Messaging;

namespace Liberfy.Messaging
{
    public class InformationDialogMessage : InteractionMessage
    {
        public InformationDialogMessage() : base()
        {
        }

        public InformationDialogMessage(string messageKey) : base()
        {
            this.MessageKey = messageKey;
        }

        protected override Freezable CreateInstanceCore() => new InformationDialogMessage(this.MessageKey)
        {
            Caption = this.Caption,
            Text = this.Text,
            Icon = this.Icon,
            Heading = this.Heading,
            ExpanderText = this.ExpanderText,
            AllowCancel = this.AllowCancel,
            AllowMimimize = this.AllowMimimize,
            IsExpanded = this.IsExpanded,
            ExpanderPosition = this.ExpanderPosition,
            FootNoteIcon = this.FootNoteIcon,
            FootNoteText = this.FootNoteText,
        };


        /// <summary>
        /// ダイアログのタイトルを取得または設定する
        /// </summary>
        public string Caption
        {
            get => this.GetValue(CaptionProperty) as string;
            set => this.SetValue(CaptionProperty, value);
        }

        /// <summary>
        /// <see cref="Caption"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(nameof(Caption), typeof(string), typeof(InformationDialogMessage), new PropertyMetadata(null));

        /// <summary>
        /// ダイアログのテキストを取得または設定する
        /// </summary>
        public string Text
        {
            get => this.GetValue(TextProperty) as string;
            set => this.SetValue(TextProperty, value);
        }

        /// <summary>
        /// ダイアログの本文のタイトルを取得または設定する
        /// </summary>
        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        /// <summary>
        /// <see cref="Heading"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty HeadingProperty =
            DependencyProperty.Register("Heading", typeof(string), typeof(InformationDialogMessage), new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Text"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(InformationDialogMessage), new PropertyMetadata(null));

        /// <summary>
        /// ダイアログのアイコンを取得または設定する
        /// </summary>
        public TaskDialogIcon? Icon
        {
            get => this.GetValue(IconProperty) as TaskDialogIcon;
            set => this.SetValue(IconProperty, value);
        }

        /// <summary>
        /// <see cref="Icon"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(TaskDialogIcon), typeof(InformationDialogMessage), new PropertyMetadata(null));



        public bool AllowCancel
        {
            get => (bool)this.GetValue(AllowCancelProperty);
            set => this.SetValue(AllowCancelProperty, value);
        }

        /// <summary>
        /// <see cref="AllowCancel"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty AllowCancelProperty =
            DependencyProperty.Register(nameof(AllowCancel), typeof(bool), typeof(InformationDialogMessage), new PropertyMetadata(true));

        public bool AllowMimimize
        {
            get => (bool)this.GetValue(AllowMinimizeProperty);
            set => this.SetValue(AllowMinimizeProperty, value);
        }

        /// <summary>
        /// <see cref="AllowMimimize"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty AllowMinimizeProperty =
            DependencyProperty.Register(nameof(AllowMimimize), typeof(bool), typeof(InformationDialogMessage), new PropertyMetadata(false));



        /// <summary>
        /// 埋め込みテキストを取得または設定する
        /// </summary>
        public string ExpanderText
        {
            get => this.GetValue(ExpanderTextProperty) as string;
            set => this.SetValue(ExpanderTextProperty, value);
        }

        /// <summary>
        /// <see cref="ExpanderText"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty ExpanderTextProperty =
            DependencyProperty.Register(nameof(ExpanderText), typeof(string), typeof(InformationDialogMessage), new PropertyMetadata(null));



        /// <summary>
        /// 埋め込みテキストの初期表示を非表示にするかどうかを取得または設定する
        /// </summary>
        public bool IsExpanded
        {
            get => (bool)this.GetValue(IsExpandedProperty);
            set => this.SetValue(IsExpandedProperty, value);
        }

        /// <summary>
        /// <see cref="IsExpanded"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(InformationDialogMessage), new PropertyMetadata(false));


        /// <summary>
        /// 埋め込みテキストの表示位置を取得または設定する
        /// </summary>
        public TaskDialogExpanderPosition ExpanderPosition
        {
            get => (TaskDialogExpanderPosition)this.GetValue(expanderPositionProperty);
            set => this.SetValue(expanderPositionProperty, value);
        }

        /// <summary>
        /// <see cref="ExpanderPosition"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty expanderPositionProperty =
            DependencyProperty.Register(nameof(ExpanderPosition), typeof(TaskDialogExpanderPosition), typeof(InformationDialogMessage), new PropertyMetadata(default(TaskDialogExpanderPosition)));


        /// <summary>
        /// フッターに表示する内容を取得または設定する
        /// </summary>
        public string FootNoteText
        {
            get => this.GetValue(FootNoteTextProperty) as string;
            set => this.SetValue(FootNoteTextProperty, value);
        }

        /// <summary>
        /// <see cref="FootNoteText"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty FootNoteTextProperty =
            DependencyProperty.Register(nameof(FootNoteText), typeof(string), typeof(InformationDialogMessage), new PropertyMetadata(null));

        /// <summary>
        /// フッターのアイコンを取得または設定する
        /// </summary>
        public TaskDialogIcon FootNoteIcon
        {
            get => this.GetValue(taskDialogIconProperty) as TaskDialogIcon;
            set => this.SetValue(taskDialogIconProperty, value);
        }

        /// <summary>
        /// <see cref="FootNoteIcon"/>のプロパティ
        /// </summary>
        public static readonly DependencyProperty taskDialogIconProperty =
            DependencyProperty.Register(nameof(FootNoteIcon), typeof(TaskDialogIcon), typeof(InformationDialogMessage), new PropertyMetadata(null));
    }
}
