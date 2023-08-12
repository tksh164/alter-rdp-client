using System;
using System.Globalization;
using System.Windows.Data;
using MsRdcAx;

namespace AlterApp.Converters
{
    [ValueConversion(typeof(RdpClientConnectionState), typeof(string))]
    internal class RdpClientConnectionStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            var state = (RdpClientConnectionState)value;
            return state switch
            {
                RdpClientConnectionState.NotConnected => "Not connected",
                RdpClientConnectionState.Connected => "Connected",
                RdpClientConnectionState.EstablishingConnection => "Establishing a connection",
                _ => string.Format("Unknown ({0})", (int)state),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
