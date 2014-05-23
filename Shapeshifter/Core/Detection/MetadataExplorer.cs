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

        void ISerializableTypeVisitor.VisitSerializableClass(SerializableTypeInfo serializableTypeInfo)
        {
            _serializers.Add(new DefaultSerializer(serializableTypeInfo));
            _deserializers.Add(new DefaultDeserializer(serializableTypeInfo));
        }

        void ISerializableTypeVisitor.VisitSerializerMethod(SerializerAttribute attribute, MethodInfo methodInfo)
        {
            if (!IsCorrectSignatureForCustomSerializer(methodInfo, attribute.TargetType))
                throw Exceptions.InvalidSerializerMethodSignature(attribute, methodInfo, attribute.TargetType);

            _serializers.Add(new CustomSerializer(attribute.TargetType, attribute.Version, methodInfo));

            if (attribute.ForAllDescendants)
            {
                var descendants = GetAllDescendants(attribute.TargetType);
                foreach (var descendant in descendants)
                {
                    _serializers.Add(new CustomSerializer(descendant, attribute.Version, methodInfo, attribute.TargetType));
                }
            }
        }

        void ISerializableTypeVisitor.VisitDeserializerMethod(DeserializerAttribute attribute, MethodInfo methodInfo)
        {
            _deserializers.Add(new CustomDeserializer(attribute.PackformatName, attribute.Version, methodInfo));

            if (attribute.ForAllDescendants)
            {
                if (attribute.TargeType == null)
                    throw Exceptions.DeserializerAttributeTargetTypeMustBeSpecifiedForAllDescendants(attribute, methodInfo);

                if (!IsCorrectSignatureForCustomDeserializerForAllDescendants(methodInfo))
                    throw Exceptions.InvalidDeserializerMethodSignatureForAllDescendants(attribute, methodInfo);

                var descendants = GetAllDescendants(attribute.TargeType);
                foreach (var descendant in descendants)
                {
                    _deserializers.Add(new CustomDeserializer(descendant.Name, attribute.Version, methodInfo, descendant));
                }
            }
            else
            {
                if (!IsCorrectSignatureForCustomDeserializer(methodInfo))
                    throw Exceptions.InvalidDeserializerMethodSignature(attribute, methodInfo);
            }
        }

        private static IEnumerable<Type> GetAllDescendants(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(i => i.GetTypes()).Where(i => i.IsSubclassOf(type));
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
                   && parameters[1].ParameterType == targetType;
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