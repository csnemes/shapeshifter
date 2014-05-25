namespace Shapeshifter.Core.Deserialization
{
    internal class ShapeshifterReader : IShapeshifterReader
    {
        private readonly ObjectProperties _elements;

        public ShapeshifterReader(ObjectProperties elements)
        {
            _elements = elements;
        }

        public uint Version
        {
            get { return _elements.Version; }
        }

        public T Read<T>(string key)
        {
            object jsonValue = _elements[key];
            return ValueConverter.ConvertValueToTargetType<T>(jsonValue);
        }

        public IShapeshifterReader GetReader(string key)
        {
            var packedInstance = _elements[key] as ObjectInPackedForm;
            if (packedInstance == null)
            {
                throw Exceptions.TheValueForTheKeyIsNotAnObject(key);
            }
            return new ShapeshifterReader(packedInstance.Elements);
        }
    }
}