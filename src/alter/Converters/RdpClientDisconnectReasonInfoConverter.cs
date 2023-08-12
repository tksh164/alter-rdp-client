using System;
using System.Globalization;
using System.Windows.Data;
using MsRdcAx;

namespace AlterApp.Converters
{
    [ValueConversion(typeof(RdpClientDisconnectReason), typeof(string))]
    internal class RdpClientDisconnectReasonInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            var disconnectReason = (RdpClientDisconnectReason)value;
            return string.Format("Reason: 0x{0:X} ({1}), ExtendedReason: 0x{2:X} ({3})", (int)disconnectReason.Reason, disconnectReason.Reason.ToString(), (int)disconnectReason.ExtendedReason, disconnectReason.ExtendedReason.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
