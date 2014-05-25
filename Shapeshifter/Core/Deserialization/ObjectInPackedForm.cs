using System;

namespace Shapeshifter.Core.Deserialization
{
    /// <summary>
    ///     Represents data and type information not yet deserialized. It contains both the data to be deserialized and the
    ///     deserializer function.
    ///     Used for defering the deserialization.
    /// </summary>
    internal class ObjectInPackedForm
    {
        private readonly Func<ObjectProperties, object> _deserializer;
        private readonly ObjectProperties _internalElements;

        public ObjectInPackedForm(ObjectProperties internalElements,
            Func<ObjectProperties, object> deserializer)
        {
            _internalElements = internalElements;
            _deserializer = deserializer;
        }

        public string PackedTypeName
        {
            get { return _internalElements.TypeName; }
        }

        public uint Version
        {
            get { return _internalElements.Version; }
        }

        public ObjectProperties Elements
        {
            get { return _internalElements; }
        }

        public object Deserialize()
        {
            return _deserializer(_internalElements);
        }
    }
}