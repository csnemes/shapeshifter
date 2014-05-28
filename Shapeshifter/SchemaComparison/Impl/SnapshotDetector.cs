﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                return new DefaultSerializerInfo(serializer.PackformatName, serializer.Version, serializer.Type.FullName);
            }
            if (serializer is CustomSerializer)
            {
                var customerSerializer = serializer as CustomSerializer;

                return new CustomSerializerInfo(serializer.PackformatName, serializer.Version, customerSerializer.MethodInfo.Name, 
                    customerSerializer.MethodInfo.DeclaringType.FullName);
            }
            throw new Exception(string.Format("Unexpected serializer type {0}.", serializer.GetType().Name));
        }

        private static DeserializerInfo ToDeserializerInfo(Deserializer deserializer)
        {
            if (deserializer is DefaultDeserializer)
            {
                var defaultDeserializer = deserializer as DefaultDeserializer;
                return new DefaultDeserializerInfo(deserializer.PackformatName, deserializer.Version, defaultDeserializer.Type.FullName);
            }
            if (deserializer is CustomDeserializer)
            {
                var customerDeserializer = deserializer as CustomDeserializer;

                return new CustomDeserializerInfo(deserializer.PackformatName, deserializer.Version, customerDeserializer.MethodInfo.Name,
                    customerDeserializer.MethodInfo.DeclaringType.FullName);
            }
            throw new Exception(string.Format("Unexpected deserializer type {0}.", deserializer.GetType().Name));
        }
    }
}