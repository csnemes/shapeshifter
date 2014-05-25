using System;
using System.Collections.Generic;

namespace Shapeshifter.Core.Serialization
{
    /// <summary>
    ///     Class containing information on types that are serializable to/from packformat
    /// </summary>
    internal class SerializerCollection
    {
        private readonly Dictionary<Type, Serializer> _serializers;

        private SerializerCollection(Dictionary<Type, Serializer> serializers)
        {
            _serializers = serializers;
        }

        public static SerializerCollectionBuilder New
        {
            get { return new SerializerCollectionBuilder(); }
        }

        public Serializer ResolveSerializer(Type type)
        {
            if (!_serializers.ContainsKey(type))
            {
                throw Exceptions.SerializerResolutionFailed(type);
            }
            return _serializers[type];
        }

        internal class SerializerCollectionBuilder
        {
            private readonly Dictionary<Type, Serializer> _serializers =
                new Dictionary<Type, Serializer>();

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

                throw Exceptions.SerializerAlreadyExists(serializer);
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