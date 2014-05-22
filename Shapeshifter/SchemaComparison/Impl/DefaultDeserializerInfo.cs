using System.Runtime.Serialization;

namespace Shapeshifter.SchemaComparison.Impl
{
    [DataContract]
    internal class DefaultDeserializerInfo : DeserializerInfo
    {
        public DefaultDeserializerInfo(string packedTypeName, uint version) : base(packedTypeName, version)
        {}
    }
}
