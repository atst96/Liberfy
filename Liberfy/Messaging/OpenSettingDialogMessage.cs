using System.Windows;
using Livet.Messaging;

namespace Liberfy.Messaging
{
    /// <summary>
    /// 設定ダイアログを表示する相互作用メッセージ。
    /// </summary>
    internal class OpenSettingDialogMessage : InteractionMessage
    {
        /// <summary>
        /// 表示するタブページのインデックスを指定する。
        /// </summary>
        public int? TabPageIndex
        {
            get => (int?)this.GetValue(TabPageIndexProperty);
            set => this.SetValue(TabPageIndexProperty, value);
        }

        // Using a DependencyProperty as the backing store for TabPageIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabPageIndexProperty =
            DependencyProperty.Register("TabPageIndex", typeof(int?), typeof(OpenSettingDialogMessage), new PropertyMetadata(null));
    }
}
