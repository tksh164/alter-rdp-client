using System;
using MsRdcAx.AxMsTscLib;

namespace MsRdcAx
{
    internal class AxMsRdpClient : AxMsRdpClient9NotSafeForScripting
    {
        public AxMsRdpClient() : base()
        {
        }

        public double GetDesktopScaleFactor()
        {
            const double nonScaledDpi = 96.0;  // DPI for 100%
            return this.DeviceDpi / nonScaledDpi;
        }

        public void SetRdpExtendedSetting(string propertyName, object propertyValue)
        {
            var rdpExtendedSettings = this.GetOcx() as MSTSCLib.IMsRdpExtendedSettings;
            if (rdpExtendedSettings != null)
            {
                rdpExtendedSettings.set_Property(propertyName, ref propertyValue);
            }
            else
            {
                throw new InvalidOperationException($@"Failed to cast to IMsRdpExtendedSettings for property ""{propertyName}"".");
            }
        }
    }
}
