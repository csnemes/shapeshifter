using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IEnumerable<Assembly> _descendantSearchScope; 
        private readonly DeserializerCollection.DeserializerCollectionBuilder _deserializers = DeserializerCollection.New;
        private readonly SerializerCollection.SerializerCollectionBuilder _serializers = SerializerCollection.New;
        private readonly SerializationStructureWalker _walker;

        private MetadataExplorer(IEnumerable<Assembly> descendantSearchScope = null)
        {
            _descendantSearchScope = descendantSearchScope ?? Enumerable.Empty<Assembly>();
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

        void ISerializableTypeVisitor.VisitSerializableClass(SerializableTypeInfo serializableTypeInfo)
        {
            _serializers.Add(new DefaultSerializer(serializableTypeInfo));
            _deserializers.Add(new DefaultDeserializer(serializableTypeInfo));
        }

        void ISerializableTypeVisitor.VisitSerializerMethod(SerializerAttribute attribute, MethodInfo methodInfo)
        {
            if (!IsCorrectSignatureForCustomSerializer(methodInfo, attribute.TargetType))
                throw Exceptions.InvalidSerializerMethodSignature(attribute, methodInfo, attribute.TargetType);

            _serializers.Add(new CustomSerializer(attribute.TargetType, attribute.PackformatName, attribute.Version, methodInfo, 
                CustomSerializerCreationReason.Explicit));

            if (attribute.ForAllDescendants)
            {
                var descendants = GetAllDescendants(attribute.TargetType);
                foreach (var descendant in descendants)
                {
                    _serializers.Add(new CustomSerializer(descendant, attribute.PackformatName, attribute.Version, methodInfo,
                        CustomSerializerCreationReason.ImplicitByBaseType));
                }
            }
        }

        void ISerializableTypeVisitor.VisitDeserializerMethod(DeserializerAttribute attribute, MethodInfo methodInfo)
        {
            _deserializers.Add(new CustomDeserializer(attribute.PackformatName, attribute.Version, methodInfo, CustomSerializerCreationReason.Explicit));

            if (attribute.ForAllDescendants)
            {
                if (attribute.TargeType == null)
                    throw Exceptions.DeserializerAttributeTargetTypeMustBeSpecifiedForAllDescendants(attribute, methodInfo);

                if (!IsCorrectSignatureForCustomDeserializerForAllDescendants(methodInfo))
                    throw Exceptions.InvalidDeserializerMethodSignatureForAllDescendants(attribute, methodInfo);

                var descendantTypes = GetAllDescendants(attribute.TargeType);
                foreach (var descendantType in descendantTypes)
                {
                    _deserializers.Add(new CustomDeserializer(descendantType.GetPrettyName(), attribute.Version, methodInfo,
                        CustomSerializerCreationReason.ImplicitByBaseType, descendantType));
                }
            }
            else
            {
                if (!IsCorrectSignatureForCustomDeserializer(methodInfo))
                    throw Exceptions.InvalidDeserializerMethodSignature(attribute, methodInfo);
            }
        }

        private IEnumerable<Type> GetAllDescendants(Type baseType)
        {
            // TODO optimize this lookup ?
            return _descendantSearchScope.SelectMany(i => i.GetTypes())
                .Where(i => i.GetBaseTypes().Any(b => b.IsSameAsOrConstructedFrom(baseType)));
        }

        public static MetadataExplorer CreateFor(Type rootType, IEnumerable<Assembly> descendantSearchScope = null)
        {
            return CreateFor(new[] {rootType}, descendantSearchScope);
        }

        public static MetadataExplorer CreateFor(IEnumerable<Type> rootTypes, IEnumerable<Assembly> descendantSearchScope = null)
        {
            return CreateFor(rootTypes, new Type[0], descendantSearchScope);
        }

        public static MetadataExplorer CreateFor(IEnumerable<Type> rootTypes, IEnumerable<Type> knownTypes, IEnumerable<Assembly> descendantSearchScope = null)
        {
            var builder = new MetadataExplorer(descendantSearchScope);
            builder.WalkRootTypes(rootTypes);
            builder.WalkKnownTypes(knownTypes);
            return builder;
        }

        private void WalkRootType(Type type)
        {
            CheckIfShapeshifterAttributePresent(type);
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

        private static void CheckIfShapeshifterAttributePresent(Type type)
        {
            var ti = new TypeInspector(type);
            if (ti.HasDataContractAttribute && ! ti.HasShapeshifterAttribute)
            {
                throw Exceptions.ShapeshifterAttributeMissing(type);
            }
        }

        private static bool IsCorrectSignatureForCustomSerializer(MethodInfo methodInfo, Type targetType)
        {
            var parameters = methodInfo.GetParameters();

            return methodInfo.ReturnParameter != null
                   && methodInfo.ReturnParameter.ParameterType == typeof(void)
                   && parameters.Length == 2
                   && parameters[0].ParameterType == typeof(IShapeshifterWriter)
                   && parameters[1].ParameterType.IsSameAsOrConstructedFrom(targetType);
        }

        private static bool IsCorrectSignatureForCustomDeserializer(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            return methodInfo.ReturnParameter != null
                   && methodInfo.ReturnParameter.ParameterType != typeof(void)
                   && parameters.Length == 1
                   && parameters[0].ParameterType == typeof(IShapeshifterReader);
        }

        private static bool IsCorrectSignatureForCustomDeserializerForAllDescendants(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            return methodInfo.ReturnParameter != null
                   && methodInfo.ReturnParameter.ParameterType != typeof(void)
                   && parameters.Length == 2
                   && parameters[0].ParameterType == typeof(IShapeshifterReader)
                   && parameters[1].ParameterType == typeof(Type);
        }
    }
}