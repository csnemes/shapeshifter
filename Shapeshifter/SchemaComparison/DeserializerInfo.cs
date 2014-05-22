using System.Runtime.Serialization;
using Shapeshifter.Core;
using Shapeshifter.Core.Deserialization;

namespace Shapeshifter.SchemaComparison
{
    [DataContract]
    internal class DeserializerInfo
    {
        [DataMember] private readonly string _packedTypeName;
        [DataMember] private readonly uint _version;

        public DeserializerInfo(string packedTypeName, uint version)
        {
            _packedTypeName = packedTypeName;
            _version = version;
        }

        public string PackedTypeName
        {
            get { return _packedTypeName; }
        }

        public uint Version
        {
            get { return _version; }
        }

        public DeserializerKey Key
        {
            get { return new DeserializerKey(_packedTypeName, _version); }
        }
    }
}