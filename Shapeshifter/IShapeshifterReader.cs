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
        T Read<T>(string key);

        /// <summary>
        ///     Returns the value corresponding to the given key
        /// </summary>
        object Read(Type type, string key);

        /// <summary>
        ///     Returns a <see cref="IShapeshifterReader" /> for the given key. Can be used to reach nested instances. Throws an exception if the value corresponding to the 
        ///     key is not an object.
        /// </summary>
        IShapeshifterReader GetReader(string key);
    }
}