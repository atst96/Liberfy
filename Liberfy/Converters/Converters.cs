using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Liberfy.Converter
{
	[ValueConversion(typeof(FontFamily), typeof(string))]
	class LocalFontNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var ff = value as FontFamily;

			if(ff != null)
			{
				if (ff.FamilyNames.ContainsKey(OSInfo.XmlLanguage))
				{
					return ff.FamilyNames[OSInfo.XmlLanguage];
				}
				{
					return ff.ToString();
				}
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
