using System.Runtime.Serialization;

namespace Shapeshifter.SchemaComparison.Impl
{
    [DataContract]
    internal class CustomSerializerInfo : SerializerInfo
    {
        [DataMember]
        private readonly string _methodName;
        [DataMember]
        private readonly string _ownerTypeFullName;

        public CustomSerializerInfo(string packformatName, uint version, string methodName, string ownerTypeFullName) : base(packformatName, version)
        {
            _methodName = methodName;
            _ownerTypeFullName = ownerTypeFullName;
        }

        public string MethodName
        {
            get { return _methodName; }
        }

        public string OwnerTypeFullName
        {
            get { return _ownerTypeFullName; }
        }
    }
}
