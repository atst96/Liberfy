using Liberfy.ViewModel;
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

            if (fontFamily != null)
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

    [ValueConversion(typeof(ColumnOptionBase), typeof(IColumn))]
    internal class ColumnSettingToColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ColumnBase.TryFromSetting(value as ColumnOptionBase, null, out var column) ? column : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as IColumn)?.GetOption();
        }
    }

    [ValueConversion(typeof(DateTimeOffset), typeof(string))]
    public class LocalTimeConverter : IValueConverter
    {
        private const string FormatFullDateTime = "yyyy年M月d日 H時m分";
        private const string FormatDateTime = "M月d日 H時mm分";
        private const string FormatTiem = "H時mm分";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTimeOffset offsetTime)
            {
                var now = DateTime.Now;
                var localTime = offsetTime.LocalDateTime;
                var time = now - offsetTime + TimeSpan.FromSeconds(1);

                //if (App.Setting.TimelineStatusShowRelativeTime)
                //{
                //	return
                //		time.TotalSeconds < 3 ? "現在"
                //		: time.TotalSeconds < 60 ? $"{time.Seconds}秒"
                //		: time.TotalMinutes < 60 ? $"{time.Minutes}分"
                //		: time.TotalHours < 24 ? $"{time.Hours}時間"
                //		: time.TotalDays > 365 ? $"{time.Days / 365}年"
                //		: time.TotalDays > 7 ? $"{time.Days / 7}週間"
                //		: $"{time.Days}日";
                //}
                //else
                //{
                return offsetTime.LocalDateTime.ToString(GetFormat(ref now, ref localTime));
                //}
            }
            else
                return DependencyProperty.UnsetValue;
        }

        private static string GetFormat(ref DateTime now, ref DateTime localTime)
        {
            if (now.Year != localTime.Year)
            {
                return FormatFullDateTime;
            }
            else if (now.Date != localTime.Date)
            {
                return FormatDateTime;
            }
            else
            {
                return FormatTiem;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
