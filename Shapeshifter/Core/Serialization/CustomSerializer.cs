using System;
using System.Reflection;

namespace Shapeshifter.Core.Serialization
{
    internal class CustomSerializer : Serializer
    {
        private readonly MethodInfo _methodInfo;

        public CustomSerializer(Type type, uint version, MethodInfo methodInfo) : base(type, version)
        {
            _methodInfo = methodInfo;
        }

        public override Action<InternalPackformatWriter, object> GetSerializerFunc()
        {
            return (writer, obj) =>
            {
                writer.WriteProperty(Constants.TypeNameKey, Type.Name);
                writer.WriteProperty(Constants.VersionKey, Version);
                _methodInfo.Invoke(null, new[] {new ShapeshifterWriter(writer), obj});
            };
        }
    }
}