using System;
using System.Reflection;

namespace Shapeshifter.Core.Deserialization
{
    internal class CustomDeserializer : Deserializer
    {
        private readonly MethodBase _methodInfo;
        private readonly CustomSerializerCreationReason _creationReason;
        private readonly Type _targetType;

        public CustomDeserializer(string packformatName, uint version, MethodBase methodInfo,
            CustomSerializerCreationReason creationReason = CustomSerializerCreationReason.Explicit, Type targetType = null)
            : base(packformatName, version)
        {
            if (creationReason == CustomSerializerCreationReason.ImplicitByBaseType && targetType == null)
                throw new ArgumentNullException("targetType", "If the creation reason is ImplicitByBaseType then targetType cannot be null.");

            if (creationReason == CustomSerializerCreationReason.Explicit && targetType != null)
                throw new ArgumentException("If the creation reason is Explicit then targetType should not be specified.", "targetType");

            _methodInfo = methodInfo;
            _creationReason = creationReason;
            _targetType = targetType;
        }

        public MethodBase MethodInfo
        {
            get { return _methodInfo; }
        }

        public CustomSerializerCreationReason CreationReason
        {
            get { return _creationReason; }
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