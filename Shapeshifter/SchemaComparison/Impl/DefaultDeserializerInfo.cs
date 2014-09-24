using System;
using System.Runtime.Serialization;

namespace Shapeshifter.SchemaComparison.Impl
{
    [DataContract]
    [Serializable]
    internal class DefaultDeserializerInfo : DeserializerInfo
    {
        [DataMember]
        private readonly string _typeFullName;

        public DefaultDeserializerInfo(string packformatName, uint version, string typeFullName) : base(packformatName, version)
        {
            _typeFullName = typeFullName;
        }

        public string TypeFullName
        {
            get { return _typeFullName; }
        }
    }
}
