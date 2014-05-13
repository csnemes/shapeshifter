namespace Shapeshifter.Core
{
    internal class PackformatValueWriterWrap : IPackformatValueWriter
    {
        private readonly IPackformatWriter _writer;

        public PackformatValueWriterWrap(IPackformatWriter writer)
        {
            _writer = writer;
        }

        public void SetValue(string key, object value)
        {
            _writer.WriteProperty(key, value);
        }
    }
}