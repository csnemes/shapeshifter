using System;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     Interface for converting values to and from serialized form
    /// </summary>
    internal interface IValueConverter
    {
        /// <summary>
        ///     Converts the given value to a serializable value (like a surrogate)
        /// </summary>
        object ConvertToPackformat(object value);

        /// <summary>
        ///     Converts the serialized value to targetType
        /// </summary>
        /// <param name="conversionHelpers">Helper functions for conversions</param>
        /// <param name="targetType">the destination type</param>
        /// <param name="value">the value read (surrogate)</param>
        /// <returns></returns>
        object ConvertFromPackformat(ConversionHelpers conversionHelpers, Type targetType, object value);

        /// <summary>
        ///     Should return true if the implementation supports the given type
        /// </summary>
        bool CanConvert(Type type);
    }
}