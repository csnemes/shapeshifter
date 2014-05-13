using System;
using System.Collections.Generic;
using System.Reflection;
using Shapeshifter.Core;

namespace Shapeshifter.History
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
            _deserializers.Add(new DeserializerInfo(attribute.PackformatName, attribute.Version));
        }

        void ISerializableTypeVisitor.VisitSerializerOnClass(TypeInfo typeInfo)
        {
            _serializers.Add(new SerializerInfo(typeInfo.PackformatName, typeInfo.Version, typeInfo.Type.FullName,
                String.Empty));
        }

        void ISerializableTypeVisitor.VisitDeserializerMethod(DeserializerAttribute attribute, MethodInfo method)
        {
            _deserializers.Add(new DeserializerInfo(attribute.PackformatName, attribute.Version));
        }

        void ISerializableTypeVisitor.VisitSerializerMethod(SerializerAttribute attribute, MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            _serializers.Add(new SerializerInfo(attribute.PackformatName, attribute.Version,
                method.DeclaringType.FullName, method.Name));
        }

        public static SnapshotDetector CreateFor(Type type)
        {
            var builder = new SnapshotDetector();
            builder.Walker.WalkType(type);
            return builder;
        }

        public static SnapshotDetector CreateFor(IEnumerable<Type> types)
        {
            var builder = new SnapshotDetector();
            builder.Walker.WalkTypes(types);
            return builder;
        }

        public static SnapshotDetector CreateFor(Assembly assembly)
        {
            var builder = new SnapshotDetector();
            builder.Walker.WalkTypes(assembly.GetTypes());
            return builder;
        }

        public static SnapshotDetector CreateFor(IEnumerable<Assembly> assemblies)
        {
            var builder = new SnapshotDetector();
            foreach (Assembly assembly in assemblies)
            {
                builder.Walker.WalkTypes(assembly.GetTypes());
            }
            return builder;
        }
    }
}