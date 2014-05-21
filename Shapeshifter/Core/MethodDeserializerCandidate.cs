using System;
using System.Reflection;

namespace Shapeshifter.Core
{
    internal class MethodDeserializerCandidate : DeserializerCandidate
    {
        private readonly MethodInfo _methodInfo;

        public MethodDeserializerCandidate(string packformatName, uint version, MethodInfo methodInfo)
            : base(packformatName, version)
        {
            _methodInfo = methodInfo;
        }

        public override Func<ObjectProperties, ValueConverter, object> GetDeserializerFunc()
        {
            return
                (objects, convHelp) =>
                    _methodInfo.Invoke(null, new object[] {new PackformatValueReaderWrap(objects, convHelp)});
        }
    }
}