using System;
using Shapeshifter.Core.Deserialization;
using System.Runtime.Serialization;

namespace Shapeshifter.SchemaComparison.Impl
{
    [DataContract]
    [KnownType(typeof(SerializerInfo))]
    [KnownType(typeof(DeserializerInfo))]
    [Serializable]
    internal abstract class InfoBase
    {
        [DataMember] private readonly string _packformatName;
        [DataMember] private readonly uint _version;

        protected InfoBase(string packformatName, uint version)
        {
            _packformatName = packformatName;
            _version = version;
        }

        public string PackformatName
        {
            get { return _packformatName; }
        }

        public uint Version
        {
            get { return _version; }
        }

        public DeserializerKey Key
        {
            get { return new DeserializerKey(_packformatName, _version); }
        }
    }
}
