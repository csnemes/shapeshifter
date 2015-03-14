using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shapeshifter.Core;
using Shapeshifter.Core.Deserialization;
using Shapeshifter.Core.Detection;
using Shapeshifter.Core.Serialization;

namespace Shapeshifter.SchemaComparison.Impl
{
    /// <summary>
    ///     Detects all involved classes and methods to create a snapshot
    /// </summary>
    internal class SnapshotDetector
    {
        private readonly IEnumerable<SerializerInfo> _serializers;
        private readonly IEnumerable<DeserializerInfo> _deserializers;

        private SnapshotDetector(IEnumerable<SerializerInfo> serializers, IEnumerable<DeserializerInfo> deserializers)
        {
            _serializers = serializers;
            _deserializers = deserializers;
        }

        public IEnumerable<SerializerInfo> Serializers
        {
            get { return _serializers; }
        }

        public IEnumerable<DeserializerInfo> Deserializers
        {
            get { return _deserializers; }
        }

        public static SnapshotDetector CreateFor(IEnumerable<Assembly> assembliesInScope)
        {
            var metadataExplorer = MetadataExplorer.CreateFor(assembliesInScope);

            var serializers = metadataExplorer.Serializers.Select(ToSerializerInfo).ToList();
            var deserializers = metadataExplorer.Deserializers.Select(ToDeserializerInfo).ToList();

            return new SnapshotDetector(serializers, deserializers);
        }

        public static SnapshotDetector CreateFor(Type type, IEnumerable<Assembly> descendantSearchScope = null)
        {
            return CreateFor(new[] {type}, null, descendantSearchScope);
        }

        public static SnapshotDetector CreateFor(IEnumerable<Type> rootTypes, IEnumerable<Type> knownTypes, IEnumerable<Assembly> descendantSearchScope = null)
        {
            var metadataExplorer = MetadataExplorer.CreateFor(
                rootTypes ?? Enumerable.Empty<Type>(),
                knownTypes ?? Enumerable.Empty<Type>(), 
                descendantSearchScope);

            var serializers = metadataExplorer.Serializers.Select(ToSerializerInfo).ToList();
            var deserializers = metadataExplorer.Deserializers.Select(ToDeserializerInfo).ToList();

            return new SnapshotDetector(serializers, deserializers);
        }

        private static SerializerInfo ToSerializerInfo(Serializer serializer)
        {
            if (serializer is DefaultSerializer)
            {
                var typeInspector = new TypeInspector(serializer.Type);
                return new DefaultSerializerInfo(serializer.PackformatName, serializer.Version, serializer.Type.GetPrettyFullName(), typeInspector.SerializableMemberCandidates);
            }
            if (serializer is CustomSerializer)
            {
                var customSerializer = serializer as CustomSerializer;

                return new CustomSerializerInfo(serializer.PackformatName, serializer.Version, customSerializer.MethodInfo.Name, 
                    customSerializer.MethodInfo.DeclaringType.FullName);
            }

            throw new Exception(string.Format("Unexpected serializer type {0}.", serializer.GetType().Name));
        }

        private static DeserializerInfo ToDeserializerInfo(Deserializer deserializer)
        {
            if (deserializer is DefaultDeserializer)
            {
                var defaultDeserializer = deserializer as DefaultDeserializer;
                return new DefaultDeserializerInfo(deserializer.PackformatName, deserializer.Version, defaultDeserializer.Type.GetPrettyFullName());
            }
            if (deserializer is CustomDeserializer)
            {
                var customDeserializer = deserializer as CustomDeserializer;

                return new CustomDeserializerInfo(deserializer.PackformatName, deserializer.Version, customDeserializer.MethodInfo.Name,
                    customDeserializer.MethodInfo.DeclaringType.FullName);
            }
            throw new Exception(string.Format("Unexpected deserializer type {0}.", deserializer.GetType().Name));
        }
    }
}