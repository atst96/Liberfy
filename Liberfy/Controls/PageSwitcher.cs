using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
    /// <summary>
    /// ページ切り替えコントロール
    /// </summary>
    public class PageSwitcher : TabControl
    {
        static PageSwitcher()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PageSwitcher), new FrameworkPropertyMetadata(typeof(PageSwitcher)));
        }

        /// <summary>
        /// <paramref name="item"/>がコンテナ内のものかどうかを取得する。
        /// </summary>
        /// <param name="item">項目</param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PageSwitcherItem;
        }

        /// <summary>
        /// 新しい項目を取得する。
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PageSwitcherItem();
        }
    }
}
