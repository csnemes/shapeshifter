using System;

namespace Shapeshifter.Core.Serialization
{
    /// <summary>
    ///     A class wich contains a serializer for a given type and version
    /// </summary>
    internal abstract class Serializer
    {
        private readonly Type _type;
        private readonly string _packformatName;
        private readonly uint _version;

        protected Serializer(Type type, string packformatName, uint version)
        {
            _type = type;
            _packformatName = packformatName;
            _version = version;
        }

        public Type Type
        {
            get { return _type; }
        }

        public string PackformatName
        {
            get { return _packformatName ?? _type.GetPrettyName(); }
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