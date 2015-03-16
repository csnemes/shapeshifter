using System;
using System.Collections.Generic;

namespace Shapeshifter
{
    /// <summary>
    ///     Interface used by custom deserializer methods.
    /// </summary>
    public interface IShapeshifterReader : IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        ///     Returns the version of the stored instance
        /// </summary>
        uint Version { get; }

        /// <summary>
        ///     Returns the value corresponding to the given key as T
        /// </summary>
        /// <param name="key">The key sought.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>Value read converted to T</returns>
        T Read<T>(string key);

        /// <summary>
        ///     Returns the value corresponding to the given key converted to the given type
        /// </summary>
        /// <param name="key">The key sought.</param>
        /// <param name="type">The target type of the value.</param>
        /// <returns>Value read converted to the given type</returns>
        object Read(Type type, string key);

        /// <summary>
        ///     Returns a <see cref="IShapeshifterReader" /> for the given key. Can be used to reach nested instances. Throws an exception if the value corresponding to the 
        ///     key is not an object.
        /// </summary>
        /// <param name="key">The key sought.</param>
        /// <returns>A <see cref="IShapeshifterReader"/> representing the value.</returns>
        IShapeshifterReader GetReader(string key);
    }
}