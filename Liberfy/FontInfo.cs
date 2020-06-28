using System.Globalization;
using System.Windows.Markup;
using System.Windows.Media;

namespace Liberfy
{
    /// <summary>
    /// フォント情報
    /// </summary>
    internal class FontInfo
    {
        private readonly static XmlLanguage _fontDisplayLanguage = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);

        /// <summary>
        /// フォントファミリ
        /// </summary>
        public FontFamily FontFamily { get; }

        /// <summary>
        /// フォント名
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// 表示名
        /// </summary>
        public string DisplayName { get; }

        public FontInfo(FontFamily fontFamily)
        {
            this.FontFamily = fontFamily;
            this.Source = fontFamily.Source;
            this.DisplayName = fontFamily.FamilyNames.TryGetValue(_fontDisplayLanguage, out var fontName)
                ? fontName : fontFamily.Source;
        }
    }
}
