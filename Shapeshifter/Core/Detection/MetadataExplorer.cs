using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
            if (serializableTypeInfo.Type == null || serializableTypeInfo.Type.IsConcreteType())
            {
                _serializers.Add(new DefaultSerializer(serializableTypeInfo));
                _deserializers.Add(new DefaultDeserializer(serializableTypeInfo));
            }
        }

        void ISerializableTypeVisitor.VisitSerializerMethod(SerializerAttribute attribute, MethodInfo methodInfo)
        {
            if (!IsCorrectSignatureForCustomSerializer(methodInfo, attribute.TargetType))
                throw Exceptions.InvalidSerializerMethodSignature(attribute, methodInfo, attribute.TargetType);

            if (attribute.TargetType.IsConcreteType())
            {
                var version = GetVersionForCustomSerializer(attribute, attribute.TargetType);
                _serializers.Add(new CustomSerializer(attribute.TargetType, attribute.PackformatName, version, methodInfo,
                    CustomSerializerCreationReason.Explicit));
            }

            if (attribute.ForAllDescendants)
            {
                var descendantTypes = GetAllDescendants(attribute.TargetType);
                foreach (var descendantType in descendantTypes.Where(i=>i.IsConcreteType()))
                {
                    var descendantVersion = GetVersionForCustomSerializer(attribute, descendantType);
                    _serializers.Add(new CustomSerializer(descendantType, attribute.PackformatName, descendantVersion, methodInfo,
                        CustomSerializerCreationReason.ImplicitByBaseType));
                }
            }
        }

        private static uint GetVersionForCustomSerializer(SerializerAttribute attribute, Type targetType)
        {
            return attribute.Version.HasValue
                ? attribute.Version.Value
                : new TypeInspector(targetType).Version;
        }

        void ISerializableTypeVisitor.VisitDeserializerMethod(DeserializerAttribute attribute, MethodInfo methodInfo)
        {
            if (attribute.ForAllDescendants)
            {
                if (attribute.TargeType == null)
                    throw Exceptions.DeserializerAttributeTargetTypeMustBeSpecifiedForAllDescendants(attribute, methodInfo);

                if (!IsCorrectSignatureForCustomDeserializerForAllDescendants(methodInfo))
                    throw Exceptions.InvalidDeserializerMethodSignatureForAllDescendants(attribute, methodInfo);
            }
            else
            {
                if (!attribute.Version.HasValue)
                    throw Exceptions.CustomDeserializerMustSpecifyVersion(attribute, methodInfo);

                if (!IsCorrectSignatureForCustomDeserializer(methodInfo))
                    throw Exceptions.InvalidDeserializerMethodSignature(attribute, methodInfo);
            }

            if (!methodInfo.IsStatic)
            {
                var defaultPublicConstructor = methodInfo.DeclaringType.GetConstructor(Type.EmptyTypes);
                if (defaultPublicConstructor == null)
                    throw Exceptions.TypeHasNoPublicDefaultConstructor(methodInfo.DeclaringType);
            }

            if (attribute.TargeType == null || attribute.TargeType.IsConcreteType())
            {
                var version = GetVersionForCustomDeserializer(attribute, attribute.TargeType);
                _deserializers.Add(new CustomDeserializer(attribute.PackformatName, version, methodInfo, CustomSerializerCreationReason.Explicit));
            }

            if (attribute.ForAllDescendants)
            {
                var descendantTypes = GetAllDescendants(attribute.TargeType);
                foreach (var descendantType in descendantTypes.Where(i=>i.IsConcreteType()))
                {
                    var descendantVersion = GetVersionForCustomDeserializer(attribute, descendantType);
                    _deserializers.Add(new CustomDeserializer(descendantType.GetPrettyName(), descendantVersion, methodInfo,
                        CustomSerializerCreationReason.ImplicitByBaseType, descendantType));
                }
            }
        }

        private static uint GetVersionForCustomDeserializer(DeserializerAttribute attribute, Type targetType)
        {
            return attribute.Version.HasValue
                ? attribute.Version.Value
                : new TypeInspector(targetType).Version;
        }

        private IEnumerable<Type> GetAllDescendants(Type baseType)
        {
            // TODO optimize this lookup ?
            return _descendantSearchScope.SelectMany(i => i.GetTypes())
                .Where(i => i.GetBaseTypes().Any(b => b.IsSameAsOrConstructedFrom(baseType)));
        }

        public static MetadataExplorer CreateFor(Type rootType, IEnumerable<Assembly> descendantSearchScope = null)
        {
            if (rootType == null) throw new ArgumentNullException("rootType");
            
            return CreateFor(new[] {rootType}, descendantSearchScope);
        }

        public static MetadataExplorer CreateFor(IEnumerable<Type> rootTypes, IEnumerable<Assembly> descendantSearchScope = null)
        {
            if (rootTypes == null) throw new ArgumentNullException("rootTypes");

            return CreateFor(rootTypes, new Type[0], descendantSearchScope);
        }

        public static MetadataExplorer CreateFor(IEnumerable<Type> rootTypes, IEnumerable<Type> knownTypes, IEnumerable<Assembly> descendantSearchScope = null)
        {
            if (rootTypes == null) throw new ArgumentNullException("rootTypes");
            if (knownTypes == null) throw new ArgumentNullException("knownTypes");

            var builder = new MetadataExplorer(descendantSearchScope);
            builder.WalkTypes(rootTypes.Concat(knownTypes));
            return builder;
        }

        public static MetadataExplorer CreateFor(IEnumerable<Assembly> assembliesInScope)
        {
            if (assembliesInScope == null) throw new ArgumentNullException("assembliesInScope");
            
            //look for all types bearing ShapeshifterRoot attribute and use them as root types
            var rootTypes = assembliesInScope.SelectMany(assembly => assembly.GetTypes()).Where(type => type.HasAttributeOfType<ShapeshifterRootAttribute>());
            var knownTypes = assembliesInScope.SelectMany(assembly => assembly.GetTypes()).Where(HasCustomDeserializerWithoutDataContract);

            return CreateFor(rootTypes, knownTypes, assembliesInScope);
        }

        private static bool HasCustomDeserializerWithoutDataContract(Type type)
        {
            //If type contains a custom deserializer or serializer must be added to known types except if the class itself is part of a serialization hierarchy (approx. has DataContract attribute)
            var hasCustomDeserializer =  type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                   BindingFlags.Instance).Any(memberInfo => memberInfo.HasAttributeOfType<DeserializerAttribute>());

            var hasCustomSerializer =  type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                   BindingFlags.Instance).Any(memberInfo => memberInfo.HasAttributeOfType<SerializerAttribute>());

            var hasDataContract = type.HasAttributeOfType<DataContractAttribute>();

            return (hasCustomDeserializer || hasCustomSerializer) && !hasDataContract;
        }

        private void WalkType(Type type)
        {
            CheckIfShapeshifterRootAttributeIsPresent(type);
            Walker.WalkRootType(type);
        }

        private void WalkTypes(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                WalkType(type);
            }
        }

        private static void CheckIfShapeshifterRootAttributeIsPresent(Type type)
        {
            var ti = new TypeInspector(type);
            if (ti.HasDataContractAttribute && ! ti.HasShapeshifterRootAttribute)
            {
                throw Exceptions.ShapeshifterRootAttributeMissing(type);
            }
        }

        private static bool IsCorrectSignatureForCustomSerializer(MethodInfo methodInfo, Type targetType)
        {
            var parameters = methodInfo.GetParameters();

            return methodInfo.ReturnParameter != null
                   && methodInfo.ReturnParameter.ParameterType == typeof (void)
                   && parameters.Length == 2
                   && parameters[0].ParameterType == typeof (IShapeshifterWriter)
                   && (parameters[1].ParameterType == typeof (object)
                       || parameters[1].ParameterType.IsSameAsOrConstructedFrom(targetType));
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