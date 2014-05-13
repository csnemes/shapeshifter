using System;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     Represents data and type information not yet deserialized. It contains both the data to be deserialized and the
    ///     deserializer function.
    ///     Used for defering the deserialization.
    /// </summary>
    internal class ObjectInPackedForm
    {
        private readonly ConversionHelpers _conversionHelpers;
        private readonly Func<ObjectProperties, ConversionHelpers, object> _deserializer;
        private readonly ObjectProperties _internalElements;

        public ObjectInPackedForm(ObjectProperties internalElements,
            Func<ObjectProperties, ConversionHelpers, object> deserializer, ConversionHelpers conversionHelpers)
        {
            _internalElements = internalElements;
            _deserializer = deserializer;
            _conversionHelpers = conversionHelpers;
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
            return _deserializer(_internalElements, _conversionHelpers);
        }
    }
}