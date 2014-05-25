using System;
using Shapeshifter.Core.Deserialization;

namespace Shapeshifter.Core.Converters
{
    /// <summary>
    ///     An <see cref="IValueConverter" /> implementation for converting Enumerables to JSON format and back
    /// </summary>
    internal class EnumToStringConverter
    {
        public object ConvertToPackformat(object value)
        {
            var enumVal = (Enum) value;
            return enumVal.ToString("G");
        }

        public object ConvertFromPackformat(Type targetType, object value)
        {
            return Enum.Parse(targetType, value.ToString(), true);
        }

        public bool CanConvert(Type type)
        {
            return type.IsEnum;
        }
    }
}