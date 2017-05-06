using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Liberfy.Converter
{
	[ValueConversion(typeof(object), typeof(string))]
	public class LocalizeNameConverter : IValueConverter
	{
		public IReadOnlyDictionary<object, string> LocalizeDictionary { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return LocalizeDictionary[value];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(FontFamily), typeof(string))]
	class LocalFontNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var fontFamily = value as FontFamily;

			if(fontFamily != null)
			{
				return fontFamily.FamilyNames.TryGetValue(OSInfo.XmlLanguage, out var xmlLang)
					? xmlLang : fontFamily.ToString();
			}
			else
			{
				return DependencyProperty.UnsetValue;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(ListBoxItem), typeof(int))]
	class ListBoxIndexConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var item = value as DependencyObject;
			var view = ItemsControl.ItemsControlFromItemContainer(item);

			return view.ItemContainerGenerator.IndexFromContainer(item) + 1;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(bool), typeof(bool))]
	class BoolenInverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}
	}

	[ValueConversion(typeof(ColumnSetting), typeof(ColumnBase))]
	class ColumnSettingToColumnConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ColumnBase.FromSettings(value as ColumnSetting);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((ColumnBase)value).ToSetting();
		}
	}
}
