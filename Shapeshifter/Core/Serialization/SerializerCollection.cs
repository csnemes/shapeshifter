using System;
using System.Collections;
using System.Collections.Generic;

namespace Shapeshifter.Core.Serialization
{
    /// <summary>
    ///     Class containing information on types that are serializable to/from packformat
    /// </summary>
    internal class SerializerCollection :IEnumerable<Serializer>
    {
        private readonly Dictionary<Type, Serializer> _serializers;

        private SerializerCollection(Dictionary<Type, Serializer> serializers)
        {
            _serializers = serializers;
        }

        public IEnumerator<Serializer> GetEnumerator()
        {
            return _serializers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _serializers.Values.GetEnumerator();
        }

        public static SerializerCollectionBuilder New
        {
            get { return new SerializerCollectionBuilder(); }
        }

        public bool HasSerializer(Type type)
        {
            return _serializers.ContainsKey(type) ||
                    (type.IsGenericType && _serializers.ContainsKey(type.GetGenericTypeDefinition()));
        }

        public Serializer ResolveSerializer(Type type)
        {
            Serializer serializer;
            if (_serializers.TryGetValue(type, out serializer))
            {
                return serializer;
            }

            if (type.IsGenericType)
            {
                if (_serializers.TryGetValue(type.GetGenericTypeDefinition(), out serializer))
                {
                    return serializer;
                }
            }

            throw Exceptions.SerializerResolutionFailed(type);
        }

        internal class SerializerCollectionBuilder
        {
            private readonly Dictionary<Type, Serializer> _serializers = new Dictionary<Type, Serializer>();

            public SerializerCollectionBuilder Add(Serializer serializer)
            {
                if (serializer == null)
                    throw new ArgumentNullException("serializer");

                var alreadyRegisteredSerializer = GetAlreadyRegisteredSerializer(serializer.Type);
                if (alreadyRegisteredSerializer == null)
                {
                    _serializers.Add(serializer.Type, serializer);
                }
                else
                {
                    _serializers[serializer.Type] = CombineSerializers(serializer, alreadyRegisteredSerializer);
                }

                return this;
            }

            private static Serializer CombineSerializers(Serializer serializer, Serializer otherSerializer)
            {
                if (serializer is CustomSerializer && otherSerializer is DefaultSerializer)
                {
                    return serializer;
                }

                if (serializer is DefaultSerializer && otherSerializer is CustomSerializer)
                {
                    return otherSerializer;
                }

                if (serializer is CustomSerializer && otherSerializer is CustomSerializer)
                {
                    return CombineCustomSerializers(serializer as CustomSerializer, otherSerializer as CustomSerializer);
                }

                throw Exceptions.SerializerAlreadyExists(serializer);
            }

            private static CustomSerializer CombineCustomSerializers(CustomSerializer customSerializer, CustomSerializer otherCustomSerializer)
            {
                if (customSerializer.CreationReason == CustomSerializerCreationReason.Explicit
                    && otherCustomSerializer.CreationReason == CustomSerializerCreationReason.ImplicitByBaseType)
                {
                    return customSerializer;
                }

                if (customSerializer.CreationReason == CustomSerializerCreationReason.ImplicitByBaseType
                    && otherCustomSerializer.CreationReason == CustomSerializerCreationReason.Explicit)
                {
                    return otherCustomSerializer;
                }

                throw Exceptions.SerializerAlreadyExists(customSerializer);
            }

            private Serializer GetAlreadyRegisteredSerializer(Type type)
            {
                Serializer alreadyRegisteredSerializer;
                _serializers.TryGetValue(type, out alreadyRegisteredSerializer);
                return alreadyRegisteredSerializer;
            }

            public static implicit operator SerializerCollection(SerializerCollectionBuilder builder)
            {
                return new SerializerCollection(builder._serializers);
            }
        }

    }
}