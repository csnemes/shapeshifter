using System;

namespace Shapeshifter.Core
{
    internal class PackformatValueReaderWrap : IPackformatValueReader
    {
        private readonly ValueConverter _valueConverter;
        private readonly ObjectProperties _elements;

        public PackformatValueReaderWrap(ObjectProperties elements, ValueConverter valueConverter)
        {
            _elements = elements;
            _valueConverter = valueConverter;
        }

        public uint Version
        {
            get { return _elements.Version; }
        }

        public T GetValue<T>(string key)
        {
            object jsonValue = _elements[key];
            return _valueConverter.ConvertValueToTargetType<T>(jsonValue);
        }

        public IPackformatValueReader GetValueReader(string key)
        {
            var packedInstance = _elements[key] as ObjectInPackedForm;
            if (packedInstance == null)
            {
                throw Exceptions.TheValueForTheKeyIsNotAnObject(key);
            }
            return new PackformatValueReaderWrap(packedInstance.Elements, _valueConverter);
        }

        public IInstanceBuilder GetBuilderFor<T>()
        {
            throw new NotImplementedException();
        }
    }
}