using System;
using System.Collections.Generic;
using System.Reflection;
using Shapeshifter.Core;
using Shapeshifter.Core.Detection;

namespace Shapeshifter.SchemaComparison.Impl
{
    /// <summary>
    ///     Detects all involved classes and methods to create a snapshot
    /// </summary>
    internal class SnapshotDetector : ISerializableTypeVisitor
    {
        private readonly List<DeserializerInfo> _deserializers = new List<DeserializerInfo>();
        private readonly List<SerializerInfo> _serializers = new List<SerializerInfo>();
        private readonly SerializationStructureWalker _walker;

        private SnapshotDetector()
        {
            _walker = new SerializationStructureWalker(this);
        }

        public IEnumerable<SerializerInfo> Serializers
        {
            get { return _serializers; }
        }

        public IEnumerable<DeserializerInfo> Deserializers
        {
            get { return _deserializers; }
        }

        private SerializationStructureWalker Walker
        {
            get { return _walker; }
        }

        void ISerializableTypeVisitor.VisitSerializableClass(SerializableTypeInfo serializableTypeInfo)
        {
            _serializers.Add(new DefaultSerializerInfo(serializableTypeInfo.PackformatName, serializableTypeInfo.Version, serializableTypeInfo.Type.FullName));
            _deserializers.Add(new DefaultDeserializerInfo(serializableTypeInfo.PackformatName, serializableTypeInfo.Version, serializableTypeInfo.Type.FullName));
        }

        void ISerializableTypeVisitor.VisitDeserializerMethod(DeserializerAttribute attribute, MethodInfo methodInfo)
        {
            _deserializers.Add(new CustomDeserializerInfo(attribute.PackformatName, attribute.Version, methodInfo.Name, methodInfo.DeclaringType.FullName));
        }

        void ISerializableTypeVisitor.VisitSerializerMethod(SerializerAttribute attribute, MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo");
            }
            _serializers.Add(new CustomSerializerInfo(attribute.PackformatName, attribute.Version, methodInfo.Name,
                methodInfo.DeclaringType.FullName));
        }

        public static SnapshotDetector CreateFor(Type type)
        {
            var builder = new SnapshotDetector();
            builder.Walker.WalkRootType(type);
            return builder;
        }

        public static SnapshotDetector CreateFor(IEnumerable<Type> types)
        {
            var builder = new SnapshotDetector();
            builder.Walker.WalkRootTypes(types);
            return builder;
        }

        public static SnapshotDetector CreateFor(Assembly assembly)
        {
            var builder = new SnapshotDetector();
            builder.Walker.WalkRootTypes(assembly.GetTypes());
            return builder;
        }

        public static SnapshotDetector CreateFor(IEnumerable<Assembly> assemblies)
        {
            var builder = new SnapshotDetector();
            foreach (Assembly assembly in assemblies)
            {
                builder.Walker.WalkRootTypes(assembly.GetTypes());
            }
            return builder;
        }
    }
}