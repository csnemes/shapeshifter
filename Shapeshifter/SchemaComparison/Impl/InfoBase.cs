using System.Runtime.Serialization;
using Shapeshifter.Core.Deserialization;

namespace Shapeshifter.SchemaComparison.Impl
{
    [DataContract]
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
