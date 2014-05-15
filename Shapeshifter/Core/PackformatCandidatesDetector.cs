using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     This class starts from the given type and detects all types used which should be serialized/deserialized (i.e. has
    ///     a DataContract attribute).
    ///     This detector also honors KnownType attribute.
    /// </summary>
    internal class PackformatCandidatesDetector : ISerializableTypeVisitor
    {
        private readonly DeserializationCandidatesCollection.DeserializationCandidatesCollectionBuilder
            _deserializationCandidates =
                DeserializationCandidatesCollection.New;

        private readonly SerializationCandidatesCollection.SerializationCandidatesCollectionBuilder
            _serializationCandidates =
                SerializationCandidatesCollection.New;

        private readonly SerializationStructureWalker _walker;

        private PackformatCandidatesDetector()
        {
            _walker = new SerializationStructureWalker(this);
        }

        public SerializationCandidatesCollection SerializationCandidates
        {
            get { return _serializationCandidates; }
        }

        public DeserializationCandidatesCollection DeserializationCandidates
        {
            get { return _deserializationCandidates; }
        }

        private SerializationStructureWalker Walker
        {
            get { return _walker; }
        }

        void ISerializableTypeVisitor.VisitDeserializerOnClass(DeserializerAttribute attribute, TypeInfo typeInfo)
        {
            _deserializationCandidates.AddCandidate(new DefaultDeserializerCandidate(attribute.PackformatName,
                attribute.Version, typeInfo));
        }

        void ISerializableTypeVisitor.VisitSerializerOnClass(TypeInfo typeInfo)
        {
            _serializationCandidates.AddCandidate(new DefaultSerializerCandidate(typeInfo));
        }

        void ISerializableTypeVisitor.VisitDeserializerMethod(DeserializerAttribute attribute, MethodInfo method)
        {
            _deserializationCandidates.AddCandidate(new MethodDeserializerCandidate(attribute.PackformatName,
                attribute.Version, method));
        }

        void ISerializableTypeVisitor.VisitSerializerMethod(SerializerAttribute attribute, MethodInfo method)
        {
            _serializationCandidates.AddCandidate(new MethodSerializerCandidate(attribute.TargetType, attribute.Version,
                method));
        }

        public static PackformatCandidatesDetector CreateFor(Type rootType)
        {
            var builder = new PackformatCandidatesDetector();
            builder.WalkRootType(rootType);
            return builder;
        }

        public static PackformatCandidatesDetector CreateFor(IEnumerable<Type> rootTypes)
        {
            return CreateFor(rootTypes, new Type[0]);
        }

        public static PackformatCandidatesDetector CreateFor(IEnumerable<Type> rootTypes, IEnumerable<Type> knownTypes)
        {
            var builder = new PackformatCandidatesDetector();
            builder.WalkRootTypes(rootTypes);
            builder.WalkKnownTypes(knownTypes);
            return builder;
        }

        private void WalkRootType(Type type)
        {
            CheckIfSerializerAttributePresent(type);
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

        private void CheckIfSerializerAttributePresent(Type type)
        {
            var ti = new TypeInspector(type);
            if (ti.HasDataContractAttribute && ! ti.HasSerializerAttribute)
            {
                throw Exceptions.SerializerAttributeMissing(type);
            }
        }
    }
}