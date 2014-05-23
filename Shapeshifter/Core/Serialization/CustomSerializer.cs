using System;
using System.Reflection;

namespace Shapeshifter.Core.Serialization
{
    internal class CustomSerializer : Serializer
    {
        private readonly MethodInfo _methodInfo;
        private readonly Type _baseType;

        public CustomSerializer(Type type, uint version, MethodInfo methodInfo, Type baseType = null) 
            : base(type, version)
        {
            _methodInfo = methodInfo;
            _baseType = baseType;
        }

        public override Action<InternalPackformatWriter, object> GetSerializerFunc()
        {
            return (writer, obj) =>
            {
                if (_baseType != null) writer.WriteProperty(Constants.BaseNameKey, _baseType.Name);
                writer.WriteProperty(Constants.TypeNameKey, Type.Name);
                writer.WriteProperty(Constants.VersionKey, Version);
                _methodInfo.Invoke(null, new[] {new ShapeshifterWriter(writer), obj});
            };
        }
    }
}