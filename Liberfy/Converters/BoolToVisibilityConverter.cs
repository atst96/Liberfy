using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Liberfy.Converters
{
    internal enum VisibilityConvertType
    {
        VisibleCollapse,
        CollapseVisible,
        VisibleHidden,
        HiddenVisible,
    }

    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(VisibilityConvertType))]
    internal class BoolToVisibilityConverter : IValueConverter
    {
        private static VisibilityConvertType ParseType(object value)
        {
            return value switch
            {
                string stringParameter => Enum.Parse<VisibilityConvertType>(stringParameter),
                VisibilityConvertType typeValue => typeValue,
                null => VisibilityConvertType.VisibleCollapse,
                _ => throw new InvalidCastException(),
            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var convertType = ParseType(parameter);

            if (value is bool boolValue)
            {
                return convertType switch
                {
                    VisibilityConvertType.VisibleCollapse => boolValue ? Visibility.Visible : Visibility.Collapsed,
                    VisibilityConvertType.VisibleHidden => boolValue ? Visibility.Visible : Visibility.Hidden,
                    VisibilityConvertType.CollapseVisible => boolValue ? Visibility.Collapsed : Visibility.Visible,
                    VisibilityConvertType.HiddenVisible => boolValue ? Visibility.Hidden : Visibility.Visible,
                    _ => new NotSupportedException(),
                };
            }

            throw new InvalidCastException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var convertType = ParseType(parameter);

            if (value is Visibility visibility)
            {
                switch (convertType)
                {
                    case VisibilityConvertType.VisibleCollapse:
                    case VisibilityConvertType.VisibleHidden:
                        return visibility == Visibility.Visible;

                    case VisibilityConvertType.CollapseVisible:
                    case VisibilityConvertType.HiddenVisible:
                        return visibility != Visibility.Visible;

                    default:
                        throw new NotSupportedException();
                }
            }

            throw new InvalidCastException();
        }
    }
}
