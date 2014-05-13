namespace Shapeshifter
{
    /// <summary>
    ///     Interface used by custom deserializer methods.
    /// </summary>
    public interface IPackformatValueReader
    {
        /// <summary>
        ///     Returns the version of the stored instance
        /// </summary>
        uint Version { get; }

        /// <summary>
        ///     Returns the value corresponding to the given key as T
        /// </summary>
        T GetValue<T>(string key);

        /// <summary>
        ///     Returns a <see cref="IPackformatValueReader" /> for the given key
        /// </summary>
        IPackformatValueReader GetValueReader(string key);

        IInstanceBuilder GetBuilderFor<T>(); //TODO
    }
}