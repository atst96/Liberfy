using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
    /// <summary>
    /// ページ切り替えコントロールのページ項目
    /// </summary>
    public class PageSwitcherItem : TabItem
    {
        static PageSwitcherItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PageSwitcherItem),
                new FrameworkPropertyMetadata(typeof(PageSwitcherItem)));
        }
    }
}
