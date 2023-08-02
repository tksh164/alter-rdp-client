using System;

namespace MsRdcAx
{
    public static class ConvertEnumValue
    {
        public static T To<T>(int value)
        {
            return FromTo<int, T>(value);
        }

        public static T To<T>(short value)
        {
            return FromTo<short, T>(value);
        }

        private static T2 FromTo<T1, T2>(T1 value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            Type t = typeof(T2);
            if (!t.IsEnum)
            {
                throw new ArgumentException(string.Format("{0} is not an Enum type.", t.FullName), nameof(T2));
            }
            if (!Enum.IsDefined(t, value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format("The value is not a member of {0}.", t.FullName));
            }
            return (T2)(object)value;
        }
    }
}
