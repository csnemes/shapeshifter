namespace Shapeshifter
{
    /// <summary>
    ///     Interface used by custom serializer methods
    /// </summary>
    public interface IPackformatValueWriter
    {
        void SetValue(string key, object value);
    }
}