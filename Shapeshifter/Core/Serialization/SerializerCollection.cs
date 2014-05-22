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
                if (_serializers.ContainsKey(serializer.Type))
                {
                    throw Exceptions.OnlyOneSerializerAllowedForAType(serializer.Type);
                }
                _serializers.Add(serializer.Type, serializer);
            }

            public static implicit operator SerializerCollection(SerializerCollectionBuilder builder)
            {
                return new SerializerCollection(builder._serializers);
            }
        }
    }
}