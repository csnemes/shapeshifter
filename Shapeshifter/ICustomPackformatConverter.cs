using System;

namespace Shapeshifter
{
    /// <summary>
    ///     Implement this interface for custom converters
    /// </summary>
    public interface ICustomPackformatConverter
    {
        /// <summary>
        ///     Converts the given value to a serializable value (like a surrogate)
        /// </summary>
        object ConvertToPackformat(object value);

        /// <summary>
        ///     Converts the serialized value to targetType
        /// </summary>
        /// <param name="targetType">the destination type</param>
        /// <param name="value">the value read (surrogate)</param>
        /// <returns></returns>
        object ConvertFromPackformat(Type targetType, object value);

        /// <summary>
        ///     Should return true if the implementation supports the given type
        /// </summary>
        bool CanConvert(Type type);
    }
}