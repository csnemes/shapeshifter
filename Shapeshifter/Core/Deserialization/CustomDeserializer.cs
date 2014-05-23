using System;
using System.Reflection;

namespace Shapeshifter.Core.Deserialization
{
    internal class CustomDeserializer : Deserializer
    {
        private readonly MethodInfo _methodInfo;

        public CustomDeserializer(string packformatName, uint version, MethodInfo methodInfo)
            : base(packformatName, version)
        {
            _methodInfo = methodInfo;
        }

        public override Func<ObjectProperties, object> GetDeserializerFunc()
        {
            return
                (objects) =>
                    _methodInfo.Invoke(null, new object[] {new ShapeshifterReader(objects)});
        }
    }
}