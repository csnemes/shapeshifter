namespace Shapeshifter.Core.Serialization
{
    internal class ShapeshifterWriter : IShapeshifterWriter
    {
        private readonly InternalPackformatWriter _writer;

        public ShapeshifterWriter(InternalPackformatWriter writer)
        {
            _writer = writer;
        }

        public void Write(string key, object value)
        {
            _writer.WriteProperty(key, value);
        }
    }
}