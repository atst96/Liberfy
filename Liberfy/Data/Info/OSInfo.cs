using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;

namespace Liberfy
{
	class OSInfo : NotificationObject
	{
		static OSInfo()
		{
			CurrentLanguage = Thread.CurrentThread.CurrentCulture.Name;
			LocalCultureInfo = new CultureInfo(CurrentLanguage);
			XmlLanguage = XmlLanguage.GetLanguage(CurrentLanguage) ?? XmlLanguage.GetLanguage("en-US");
		}

		public static string CurrentLanguage { get; private set; }

		public static XmlLanguage XmlLanguage { get; private set; }

		public static CultureInfo LocalCultureInfo { get; private set; }

		private ICollection<FontFamily> _fontFamilies;
		public ICollection<FontFamily> FontFamilies
		{
			get { return _fontFamilies ?? (_fontFamilies = Fonts.SystemFontFamilies); }
		}

		public void RefreshFontFamilies()
		{
			_fontFamilies = Fonts.SystemFontFamilies;
			RaisePropertyChanged(nameof(FontFamilies));
		}
	}
}
