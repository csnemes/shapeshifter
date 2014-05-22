using System.IO;

namespace Shapeshifter
{
    public interface IShapeshifter
    {
        void Serialize(Stream targetStream, object objToPack);
        string Serialize(object objToPack);
        object Deserialize(Stream sourceStream);
        object Deserialize(string source);
    }

    public interface IShapeshifter<out T>
    {
        void Serialize(Stream targetStream, object objToPack);
        string Serialize(object objToPack);
        T Deserialize(Stream sourceStream);
        T Deserialize(string source);
    }
}