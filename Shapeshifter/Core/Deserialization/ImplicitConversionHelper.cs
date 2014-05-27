using System;
using System.Numerics;

namespace Shapeshifter.Core.Deserialization
{
    public static class ImplicitConversionHelper
    {
        public static object ConvertValue(Type targetType, object value)
        {
            if (value is long)
            {
                if (targetType == typeof(int)) //JSON stores int as long
                {
                    return Convert.ToInt32(value);
                }
                if (targetType == typeof(short))
                {
                    return Convert.ToInt16(value);
                }
                if (targetType == typeof(uint))
                {
                    return Convert.ToUInt32(value);
                }
                if (targetType == typeof(ushort))
                {
                    return Convert.ToUInt16(value);
                }
                if (targetType == typeof(byte))
                {
                    return Convert.ToByte(value);
                }
                if (targetType == typeof(sbyte))
                {
                    return Convert.ToSByte(value);
                }
            }
            else if (value is double)
            {
                if (targetType == typeof(float))
                {
                    return Convert.ToSingle(value);
                }
                if (targetType == typeof(decimal))
                {
                    return Convert.ToDecimal(value);
                }
            }
            else if (value is BigInteger)
            {
                if (targetType == typeof(ulong))
                {
                    return (ulong)(BigInteger)value;
                }
            }
            else if (value is string)
            {
                if (targetType == typeof(char))
                {
                    return Convert.ToChar(value);
                }
                if (targetType == typeof (Guid))
                {
                    return Guid.Parse((string) value);
                }
            }
            else if (value is DateTime)
            {
                if (targetType == typeof(DateTimeOffset))
                {
                    return new DateTimeOffset((DateTime)value);
                }
            }

            return value; //no conversion
        }
    }
}
