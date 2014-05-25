using System.Runtime.Serialization;

namespace Shapeshifter.SchemaComparison.Impl
{
    [DataContract]
    internal class DefaultSerializerInfo : SerializerInfo
    {
        [DataMember]
        private readonly string _typeFullName;

        public DefaultSerializerInfo(string packformatName, uint version, string typeFullName) : base(packformatName, version)
        {
            _typeFullName = typeFullName;
        }

        public string TypeFullName
        {
            get { return _typeFullName; }
        }
    }
}
