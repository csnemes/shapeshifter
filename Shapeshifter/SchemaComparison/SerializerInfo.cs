using System.Runtime.Serialization;
using Shapeshifter.Core;
using Shapeshifter.Core.Deserialization;

namespace Shapeshifter.SchemaComparison
{
    [DataContract]
    internal class SerializerInfo
    {
        [DataMember] private readonly string _methodName;
        [DataMember] private readonly string _packformatName;
        [DataMember] private readonly string _typeFullName;
        [DataMember] private readonly uint _version;

        public SerializerInfo(string packformatName, uint version, string typeFullName, string methodName)
        {
            _packformatName = packformatName;
            _version = version;
            _typeFullName = typeFullName;
            _methodName = methodName;
        }

        public string PackformatName
        {
            get { return _packformatName; }
        }

        public uint Version
        {
            get { return _version; }
        }

        public string TypeFullName
        {
            get { return _typeFullName; }
        }

        public DeserializerKey Key
        {
            get { return new DeserializerKey(_packformatName, _version); }
        }
    }
}