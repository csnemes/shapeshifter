namespace Shapeshifter.Core
{
    /// <summary>
    ///     Interface used when serializing an object
    /// </summary>
    internal interface IPackformatWriter
    {
        void WriteProperty(string propertyKey, object propertyValue);
    }
}