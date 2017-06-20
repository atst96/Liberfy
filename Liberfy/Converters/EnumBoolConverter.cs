using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Liberfy.Converter
{
	[ValueConversion(typeof(Enum), typeof(bool), ParameterType = typeof(string))]
	public class EnumBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value is Enum enumValue && parameter is string fieldName)
			{
				var currentName = Enum.GetName(enumValue.GetType(), enumValue);
				return string.Equals(currentName, fieldName, StringComparison.OrdinalIgnoreCase);
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value is bool isValue && parameter is string fieldName)
			{
				return Enum.Parse(targetType, fieldName);
			}

			return DependencyProperty.UnsetValue;
		}
	}
}
