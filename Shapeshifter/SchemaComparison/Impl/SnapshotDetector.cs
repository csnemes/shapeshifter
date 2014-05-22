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

        void ISerializableTypeVisitor.VisitDeserializerOnClass(DeserializerAttribute attribute, TypeInfo typeInfo)
        {
            _deserializers.Add(new DefaultDeserializerInfo(attribute.PackformatName, attribute.Version, typeInfo.Type.FullName));
        }

        void ISerializableTypeVisitor.VisitSerializerOnClass(TypeInfo typeInfo)
        {
            _serializers.Add(new DefaultSerializerInfo(typeInfo.PackformatName, typeInfo.Version, typeInfo.Type.FullName));
        }

        void ISerializableTypeVisitor.VisitDeserializerMethod(DeserializerAttribute attribute, MethodInfo method)
        {
            _deserializers.Add(new CustomDeserializerInfo(attribute.PackformatName, attribute.Version, method.Name, method.DeclaringType.FullName));
        }

        void ISerializableTypeVisitor.VisitSerializerMethod(SerializerAttribute attribute, MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            _serializers.Add(new CustomSerializerInfo(attribute.PackformatName, attribute.Version, method.Name,
                method.DeclaringType.FullName));
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