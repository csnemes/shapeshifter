using System;
using System.Collections.Generic;
using System.Reflection;
using Shapeshifter.Core.Deserialization;
using Shapeshifter.Core.Serialization;

namespace Shapeshifter.Core.Detection
{
    /// <summary>
    ///     This class starts from the given type and detects all types used which should be serialized/deserialized (i.e. has
    ///     a DataContract attribute).
    ///     This detector also honors KnownType attribute.
    /// </summary>
    internal class MetadataExplorer : ISerializableTypeVisitor
    {
        private readonly DeserializerCollection.DeserializerCollectionBuilder _deserializers = DeserializerCollection.New;
        private readonly SerializerCollection.SerializerCollectionBuilder _serializers = SerializerCollection.New;
        private readonly SerializationStructureWalker _walker;

        private MetadataExplorer()
        {
            _walker = new SerializationStructureWalker(this);
        }

        public SerializerCollection Serializers
        {
            get { return _serializers; }
        }

        public DeserializerCollection Deserializers
        {
            get { return _deserializers; }
        }

        private SerializationStructureWalker Walker
        {
            get { return _walker; }
        }

        void ISerializableTypeVisitor.VisitDeserializerOnClass(DeserializerAttribute attribute, TypeInfo typeInfo)
        {
            _deserializers.Add(new DefaultDeserializer(attribute.PackformatName, attribute.Version, typeInfo));
        }

        void ISerializableTypeVisitor.VisitSerializerOnClass(TypeInfo typeInfo)
        {
            _serializers.Add(new DefaultSerializer(typeInfo));
        }

        void ISerializableTypeVisitor.VisitDeserializerMethod(DeserializerAttribute attribute, MethodInfo method)
        {
            _deserializers.Add(new CustomDeserializer(attribute.PackformatName, attribute.Version, method));
        }

        void ISerializableTypeVisitor.VisitSerializerMethod(SerializerAttribute attribute, MethodInfo method)
        {
            _serializers.Add(new CustomSerializer(attribute.TargetType, attribute.Version, method));
        }

        public static MetadataExplorer CreateFor(Type rootType)
        {
            return CreateFor(new[] {rootType});
        }

        public static MetadataExplorer CreateFor(IEnumerable<Type> rootTypes)
        {
            return CreateFor(rootTypes, new Type[0]);
        }

        public static MetadataExplorer CreateFor(IEnumerable<Type> rootTypes, IEnumerable<Type> knownTypes)
        {
            var builder = new MetadataExplorer();
            builder.WalkRootTypes(rootTypes);
            builder.WalkKnownTypes(knownTypes);
            return builder;
        }

        private void WalkRootType(Type type)
        {
            CheckIfShapeshifterRootAttributePresent(type);
            Walker.WalkRootType(type);
        }

        private void WalkKnownType(Type type)
        {
            Walker.WalkKnownType(type);
        }

        private void WalkRootTypes(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                WalkRootType(type);
            }
        }

        private void WalkKnownTypes(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                WalkKnownType(type);
            }
        }

        private static void CheckIfShapeshifterRootAttributePresent(Type type)
        {
            var ti = new TypeInspector(type);
            if (ti.HasDataContractAttribute && ! ti.HasShapeshifterRootAttribute)
            {
                throw Exceptions.ShapeshifterRootAttributeMissing(type);
            }
        }
    }
}