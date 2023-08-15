using MsRdcAx.AxMsTscLib;

namespace MsRdcAx
{
    internal class AxMsRdpClient : AxMsRdpClient9NotSafeForScripting
    {
        public AxMsRdpClient() : base()
        {
        }

        public double GetScaleFactor()
        {
            const double nonScaledDpi = 96.0;  // DPI for 100%
            return this.DeviceDpi / nonScaledDpi;
        }
    }
}
