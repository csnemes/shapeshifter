using System.Runtime.Serialization;

namespace Shapeshifter.SchemaComparison.Impl
{
    [DataContract]
    [KnownType(typeof(CustomDeserializerInfo))]
    [KnownType(typeof(DefaultDeserializerInfo))]
    internal abstract class DeserializerInfo : InfoBase
    {
        protected DeserializerInfo(string packformatName, uint version) : base(packformatName, version)
        {
        }
    }
}