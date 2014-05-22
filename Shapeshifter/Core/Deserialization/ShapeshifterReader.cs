namespace Shapeshifter.Core.Deserialization
{
    internal class ShapeshifterReader : IShapeshifterReader
    {
        private readonly ValueConverter _valueConverter;
        private readonly ObjectProperties _elements;

        public ShapeshifterReader(ObjectProperties elements, ValueConverter valueConverter)
        {
            _elements = elements;
            _valueConverter = valueConverter;
        }

        public uint Version
        {
            get { return _elements.Version; }
        }

        public T Read<T>(string key)
        {
            object jsonValue = _elements[key];
            return _valueConverter.ConvertValueToTargetType<T>(jsonValue);
        }

        public IShapeshifterReader GetReader(string key)
        {
            var packedInstance = _elements[key] as ObjectInPackedForm;
            if (packedInstance == null)
            {
                throw Exceptions.TheValueForTheKeyIsNotAnObject(key);
            }
            return new ShapeshifterReader(packedInstance.Elements, _valueConverter);
        }
    }
}