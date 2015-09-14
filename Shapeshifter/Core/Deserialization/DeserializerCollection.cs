using System;
using System.Collections;
using System.Collections.Generic;

namespace Shapeshifter.Core.Deserialization
{
    /// <summary>
    ///     Class containing information on types that are serializable to/from packformat
    /// </summary>
    internal class DeserializerCollection:IEnumerable<Deserializer>
    {
        private readonly Dictionary<DeserializerKey, Deserializer> _deserializers;

        private DeserializerCollection(Dictionary<DeserializerKey, Deserializer> deserializers)
        {
            _deserializers = deserializers;
        }

        public IEnumerator<Deserializer> GetEnumerator()
        {
            return _deserializers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _deserializers.Values.GetEnumerator();
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

                if (deserializer is CustomDeserializer && otherDeserializer is CustomDeserializer)
                {
                    return CombineCustomDeserializers(deserializer as CustomDeserializer, otherDeserializer as CustomDeserializer);
                }

                throw Exceptions.DeserializerAlreadyExists(deserializer);
            }

            private static CustomDeserializer CombineCustomDeserializers(CustomDeserializer customDeserializer, CustomDeserializer otherCustomDeserializer)
            {
                if (customDeserializer.CreationReason == CustomSerializerCreationReason.Explicit
                    && otherCustomDeserializer.CreationReason == CustomSerializerCreationReason.ImplicitByBaseType)
                {
                    return customDeserializer;
                }

                if (customDeserializer.CreationReason == CustomSerializerCreationReason.ImplicitByBaseType
                    && otherCustomDeserializer.CreationReason == CustomSerializerCreationReason.Explicit)
                {
                    return otherCustomDeserializer;
                }

                if (customDeserializer.MethodInfo == otherCustomDeserializer.MethodInfo)
                {
                    return customDeserializer;
                }

                throw Exceptions.DeserializerAlreadyExists(customDeserializer);
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