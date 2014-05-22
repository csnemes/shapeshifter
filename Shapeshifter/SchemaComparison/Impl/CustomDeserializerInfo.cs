using System.Runtime.Serialization;

namespace Shapeshifter.SchemaComparison.Impl
{
    [DataContract]
    internal class CustomDeserializerInfo : DeserializerInfo
    {
        public CustomDeserializerInfo(string packedTypeName, uint version) : base(packedTypeName, version)
        {
        }
    }
}
