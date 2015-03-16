namespace Shapeshifter
{
    /// <summary>
    ///     Interface used by custom serializer methods
    /// </summary>
    public interface IShapeshifterWriter
    {
        /// <summary>
        /// Writes the value to the serialization stream under the given key.
        /// </summary>
        /// <param name="key">target key</param>
        /// <param name="value">value to write</param>
        void Write(string key, object value);
    }
}