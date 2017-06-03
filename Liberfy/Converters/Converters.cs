using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public class DummyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.WriteLine($"Converted: {value}");
			Debugger.Break();
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.WriteLine($"Reconverted: {value}");
			Debugger.Break();
			return value;
		}
	}

	[ValueConversion(typeof(object), typeof(string))]
	public class LocalizeNameConverter : IValueConverter
	{
		public IDictionary<object, string> LocalizeDictionary { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return LocalizeDictionary.TryGetValue(value, out var strVal)
				? strVal : DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(FontFamily), typeof(string))]
	internal class LocalFontNameConverter : IValueConverter
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
	internal class ListBoxIndexConverter : IValueConverter
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
	internal class BoolenInverter : IValueConverter
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
	internal class ColumnSettingToColumnConverter : IValueConverter
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
