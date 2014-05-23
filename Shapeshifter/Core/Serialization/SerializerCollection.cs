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

            public void Add(Serializer serializer)
            {
                Serializer alreadyRegisteredSerializer;
                if (_serializers.TryGetValue(serializer.Type, out alreadyRegisteredSerializer))
                {
                    if (serializer.GetType() == alreadyRegisteredSerializer.GetType())
                        throw Exceptions.OnlyOneSerializerAllowedForAType(serializer.Type);

                    if (NewSerializerCanOverrideAlreadyRegisteredSerializer(serializer, alreadyRegisteredSerializer))
                    {
                        _serializers[serializer.Type] = serializer;
                    }
                }
                else
                {
                    _serializers.Add(serializer.Type, serializer);
                }
            }

            private static bool NewSerializerCanOverrideAlreadyRegisteredSerializer(Serializer newSerializer, Serializer alreadyRegisteredSerializer)
            {
                return newSerializer is CustomSerializer && alreadyRegisteredSerializer is DefaultSerializer;
            }

            public static implicit operator SerializerCollection(SerializerCollectionBuilder builder)
            {
                return new SerializerCollection(builder._serializers);
            }
        }
    }
}