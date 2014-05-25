using System;
using System.Reflection;

namespace Shapeshifter.Core.Deserialization
{
    internal class CustomDeserializer : Deserializer
    {
        private readonly MethodInfo _methodInfo;
        private readonly Type _targetType;

        public CustomDeserializer(string packformatName, uint version, MethodInfo methodInfo, Type targetType = null)
            : base(packformatName, version)
        {
            _methodInfo = methodInfo;
            _targetType = targetType;
        }

        public override Func<ObjectProperties, object> GetDeserializerFunc()
        {
            return (objects) =>
                _targetType == null
                    ? _methodInfo.Invoke(null, new object[] {new ShapeshifterReader(objects)})
                    : _methodInfo.Invoke(null, new object[] {new ShapeshifterReader(objects), _targetType});
        }
    }
}