using System;

namespace Shapeshifter
{
    /// <summary>
    /// Implement this interface for custom converters
    /// The surrogate class used must be a DataContract serializable class having a DataContract attribute.
    /// The surrogate class must be added as KnownType to the implementing class using the KnownTypeAttribute.
    /// </summary>
    public interface IPackformatSurrogateConverter
    {
        /// <summary>
        ///     Converts the given value to a surrogate
        /// </summary>
        object ConvertToSurrogate(object value);

        /// <summary>
        ///     Converts the serialized value to targetType
        /// </summary>
        /// <param name="targetType">the destination type</param>
        /// <param name="value">the value read (surrogate)</param>
        /// <returns></returns>
        object ConvertFromSurrogate(Type targetType, object value);

        /// <summary>
        ///     Should return true if the implementation supports the given type
        /// </summary>
        bool CanConvert(Type type);
    }
}