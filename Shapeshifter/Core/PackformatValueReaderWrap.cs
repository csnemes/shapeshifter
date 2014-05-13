using System;

namespace Shapeshifter.Core
{
    internal class PackformatValueReaderWrap : IPackformatValueReader
    {
        private readonly ConversionHelpers _conversionHelpers;
        private readonly ObjectProperties _elements;

        public PackformatValueReaderWrap(ObjectProperties elements, ConversionHelpers conversionHelpers)
        {
            _elements = elements;
            _conversionHelpers = conversionHelpers;
        }

        public uint Version
        {
            get { return _elements.Version; }
        }

        public T GetValue<T>(string key)
        {
            object jsonValue = _elements[key];
            return _conversionHelpers.ConvertValueToTargetType<T>(jsonValue);
        }

        public IPackformatValueReader GetValueReader(string key)
        {
            var packedInstance = _elements[key] as ObjectInPackedForm;
            if (packedInstance == null)
            {
                throw Exceptions.TheValueForTheKeyIsNotAnObject(key);
            }
            return new PackformatValueReaderWrap(packedInstance.Elements, _conversionHelpers);
        }

        public IInstanceBuilder GetBuilderFor<T>()
        {
            throw new NotImplementedException();
        }
    }
}