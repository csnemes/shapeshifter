using System.Runtime.Serialization;

namespace Shapeshifter.SchemaComparison.Impl
{
    [DataContract]
    internal class CustomSerializerInfo : SerializerInfo
    {
        [DataMember]
        private readonly string _methodName;
        [DataMember]
        private readonly string _declaringTypeFullName;

        public CustomSerializerInfo(string packformatName, uint version, string methodName, string declaringTypeFullName) : base(packformatName, version)
        {
            _methodName = methodName;
            _declaringTypeFullName = declaringTypeFullName;
        }

        public string MethodName
        {
            get { return _methodName; }
        }

        public string DeclaringTypeFullName
        {
            get { return _declaringTypeFullName; }
        }
    }
}
