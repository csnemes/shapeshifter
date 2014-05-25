using System;
using System.Collections.Generic;

namespace Shapeshifter.Core.Deserialization
{
    /// <summary>
    ///     Class containing information on types that are serializable to/from packformat
    /// </summary>
    internal class DeserializerCollection
    {
        private readonly Dictionary<DeserializerKey, Deserializer> _deserializers;

        private DeserializerCollection(Dictionary<DeserializerKey, Deserializer> deserializers)
        {
            _deserializers = deserializers;
        }

        public static DeserializerCollectionBuilder New
        {
            get { return new DeserializerCollectionBuilder(); }
        }

        public Deserializer ResolveDeserializer(DeserializerKey deserializerKey)
        {
            if (deserializerKey == null)
            {
                throw new ArgumentNullException("deserializerKey");
            }

            Deserializer result;
            if (_deserializers.TryGetValue(deserializerKey, out result))
            {
                return result;
            }

            var versionNeutralDeserializerKey = new DeserializerKey(deserializerKey.PackedName);
            if (_deserializers.TryGetValue(versionNeutralDeserializerKey, out result))
            {
                return result;
            }

            return null;
        }

        internal class DeserializerCollectionBuilder
        {
            private readonly Dictionary<DeserializerKey, Deserializer> _deserializers = new Dictionary<DeserializerKey, Deserializer>();

            public DeserializerCollectionBuilder Add(Deserializer deserializer)
            {
                if (deserializer == null)
                    throw new ArgumentNullException("deserializer");

                var alreadyRegisteredDeserializer = GetAlreadyRegisteredDeserializer(deserializer.Key);
                if (alreadyRegisteredDeserializer == null)
                {
                    _deserializers.Add(deserializer.Key, deserializer);
                }
                else
                {
                    _deserializers[deserializer.Key] = CombineDeserializers(deserializer, alreadyRegisteredDeserializer);
                }

                return this;
            }

            private static Deserializer CombineDeserializers(Deserializer deserializer, Deserializer otherDeserializer)
            {
                if (deserializer is CustomDeserializer && otherDeserializer is DefaultDeserializer)
                {
                    return deserializer;
                }

                if (deserializer is DefaultDeserializer && otherDeserializer is CustomDeserializer)
                {
                    return otherDeserializer;
                }

                throw Exceptions.DeserializerAlreadyExists(deserializer);
            }

            private Deserializer GetAlreadyRegisteredDeserializer(DeserializerKey deserializerKey)
            {
                Deserializer alreadyRegisteredDeserializer;
                _deserializers.TryGetValue(deserializerKey, out alreadyRegisteredDeserializer);
                return alreadyRegisteredDeserializer;
            }

            public static implicit operator DeserializerCollection(DeserializerCollectionBuilder builder)
            {
                return new DeserializerCollection(builder._deserializers);
            }
        }
    }
}