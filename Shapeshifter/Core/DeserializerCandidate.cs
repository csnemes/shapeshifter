using System;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     A class wich contains a deserializer for a given type and version
    /// </summary>
    internal abstract class DeserializerCandidate
    {
        private readonly DeserializerKey _key;

        protected DeserializerCandidate(string packformatName, uint version)
        {
            _key = new DeserializerKey(packformatName, version);
        }

        public DeserializerKey Key
        {
            get { return _key; }
        }

        public string PackformatName
        {
            get { return _key.PackedName; }
        }

        public uint Version
        {
            get { return _key.Version; }
        }

        public bool HasVersion
        {
            get { return _key.Version != default(uint); }
        }

        /// <summary>
        ///     Override should return a function which based on the given <see cref="ObjectProperties" />, using the given
        ///     <see cref="ValueConverter" />
        ///     builds up the deserialized value.
        /// </summary>
        public abstract Func<ObjectProperties, ValueConverter, object> GetDeserializerFunc();
    }
}