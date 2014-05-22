namespace Shapeshifter
{
    /// <summary>
    ///     Interface used by custom serializer methods
    /// </summary>
    public interface IShapeshifterWriter
    {
        void Write(string key, object value);
    }
}