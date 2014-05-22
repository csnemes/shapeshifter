using System;

namespace Shapeshifter.Core.Serialization
{
    /// <summary>
    ///     A class wich contains a serializer for a given type and version
    /// </summary>
    internal abstract class Serializer
    {
        private readonly Type _type;
        private readonly uint _version;

        protected Serializer(Type type, uint version)
        {
            _type = type;
            _version = version;
        }

        public Type Type
        {
            get { return _type; }
        }

        public uint Version
        {
            get { return _version; }
        }

        /// <summary>
        ///     Overrides must use the given <see cref="InternalPackformatWriter" /> to serialize the object passed-in.
        /// </summary>
        public abstract Action<InternalPackformatWriter, object> GetSerializerFunc();
    }
}