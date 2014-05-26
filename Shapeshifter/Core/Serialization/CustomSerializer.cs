using System;
using System.Reflection;

namespace Shapeshifter.Core.Serialization
{
    internal class CustomSerializer : Serializer
    {
        private readonly MethodInfo _methodInfo;
        private readonly CustomSerializerCreationReason _creationReason;

        public CustomSerializer(Type type, string packformatName, uint version, MethodInfo methodInfo, 
            CustomSerializerCreationReason creationReason = CustomSerializerCreationReason.Explicit) 
            : base(type, packformatName, version)
        {
            _methodInfo = methodInfo;
            _creationReason = creationReason;
        }

        public CustomSerializerCreationReason CreationReason
        {
            get { return _creationReason; }
        }

        public override Action<InternalPackformatWriter, object> GetSerializerFunc()
        {
            return (writer, obj) =>
            {
                writer.WriteProperty(Constants.TypeNameKey, PackformatName);
                writer.WriteProperty(Constants.VersionKey, Version);
                _methodInfo.Invoke(null, new[] {new ShapeshifterWriter(writer), obj});
            };
        }
    }
}