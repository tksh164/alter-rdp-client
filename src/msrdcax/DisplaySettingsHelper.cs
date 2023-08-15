namespace MsRdcAx
{
    internal class DisplaySettingsHelper
    {
        public static uint ConvertToPhysicalUnitSize(uint desktopSize, uint desktopScaleFactor)
        {
            const double oneInchInMillimeter = 25.4;
            return (uint)(desktopSize / desktopScaleFactor * oneInchInMillimeter);
        }
    }
}
