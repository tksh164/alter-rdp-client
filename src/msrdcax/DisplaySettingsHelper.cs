namespace MsRdcAx
{
    internal class DisplaySettingsHelper
    {
        public static double GetDisplayScaleFactor(int deviceDpi)
        {
            const double nonScaledDpi = 96.0;  // DPI for 100%
            return deviceDpi / nonScaledDpi;
        }

        public static uint ConvertToPhysicalUnitSize(uint desktopSize, uint desktopScaleFactor)
        {
            const double oneInchInMillimeter = 25.4;
            return (uint)(desktopSize / desktopScaleFactor * oneInchInMillimeter);
        }
    }
}
