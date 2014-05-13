using System;
using System.Reflection;

namespace Shapeshifter.Core
{
    internal class MethodSerializerCandidate : SerializerCandidate
    {
        private readonly MethodInfo _methodInfo;

        public MethodSerializerCandidate(Type type, uint version, MethodInfo methodInfo) : base(type, version)
        {
            _methodInfo = methodInfo;
        }

        public override Action<IPackformatWriter, object> GetSerializerFunc()
        {
            return (writer, obj) =>
            {
                writer.WriteProperty(Constants.TypeNameKey, Type.Name);
                writer.WriteProperty(Constants.VersionKey, Version);
                _methodInfo.Invoke(null, new[] {new PackformatValueWriterWrap(writer), obj});
            };
        }
    }
}