using System;

namespace MsRdcAx
{
    public static class ConvertEnumValue
    {
        public static T To<T>(object value)
        {
            return ConvertEnumRawValueToEnumMember<T>(value);
        }

        private static T ConvertEnumRawValueToEnumMember<T>(object value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            Type t = typeof(T);
            if (!t.IsEnum)
            {
                throw new ArgumentException(string.Format("{0} is not an Enum type.", t.FullName), nameof(T));
            }
            if (!Enum.IsDefined(t, value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format("The value is not a member of {0}.", t.FullName));
            }
            return (T)value;
        }
    }
}
