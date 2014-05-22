namespace Shapeshifter
{
    /// <summary>
    ///     Interface used by custom deserializer methods.
    /// </summary>
    public interface IShapeshifterReader
    {
        /// <summary>
        ///     Returns the version of the stored instance
        /// </summary>
        uint Version { get; }

        /// <summary>
        ///     Returns the value corresponding to the given key as T
        /// </summary>
        T Read<T>(string key);

        /// <summary>
        ///     Returns a <see cref="IShapeshifterReader" /> for the given key
        /// </summary>
        IShapeshifterReader GetReader(string key);
    }
}