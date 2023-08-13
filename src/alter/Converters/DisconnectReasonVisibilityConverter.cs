using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace AlterApp.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    internal class DisconnectReasonVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
