using System;
using System.IO;

namespace Shapeshifter
{
    public interface IShapeshifter
    {
        void Serialize(Stream targetStream, object objToPack, Type declaredSourceType = null);
        string Serialize(object objToPack, Type declaredSourceType = null);
        object Deserialize(Stream sourceStream);
        object Deserialize(string source);
    }

    public interface IShapeshifter<out T>
    {
        void Serialize(Stream targetStream, object objToPack, Type declaredSourceType = null);
        string Serialize(object objToPack, Type declaredSourceType = null);
        T Deserialize(Stream sourceStream);
        T Deserialize(string source);
    }
}