using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Liberfy.Converter
{
    [ValueConversion(typeof(ServiceType), typeof(string))]
    internal class ServiceTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ServiceType serviceType))
            {
                throw new InvalidOperationException();
            }

            return serviceType switch
            {
                ServiceType.Twitter => "Twitter",
                ServiceType.Mastodon => "Mastodon",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string name))
            {
                throw new InvalidOperationException();
            }

            return (name.ToLower()) switch
            {
                "twitter" => ServiceType.Twitter,
                "mastodon" => ServiceType.Mastodon,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
