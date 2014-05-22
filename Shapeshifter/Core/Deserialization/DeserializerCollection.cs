using System;
using System.Collections.Generic;
using System.Linq;

namespace Shapeshifter.Core.Deserialization
{
    /// <summary>
    ///     Class containing information on types that are serializable to/from packformat
    /// </summary>
    internal class DeserializerCollection
    {
        private readonly Dictionary<string, List<Deserializer>> _deserializers;

        private DeserializerCollection(Dictionary<string, List<Deserializer>> deserializers)
        {
            _deserializers = deserializers;
        }

        public static DeserializerCollectionBuilder New
        {
            get { return new DeserializerCollectionBuilder(); }
        }

        public Deserializer ResolveDeserializer(string typeName, uint version)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }

            if (!_deserializers.ContainsKey(typeName))
            {
                return null;
            }

            var deserializers = _deserializers[typeName];

            var result = deserializers.FirstOrDefault(item => item.Version == version)
                         ?? deserializers.FirstOrDefault(item => item.Version == 0);

            return result;
        }


        internal class DeserializerCollectionBuilder
        {
            private readonly Dictionary<string, List<Deserializer>> _deserializers = new Dictionary<string, List<Deserializer>>();

            public void Add(Deserializer deserializer)
            {
                if (deserializer == null)
                {
                    throw new ArgumentNullException("deserializer");
                }

                if (!_deserializers.ContainsKey(deserializer.PackformatName))
                {
                    _deserializers[deserializer.PackformatName] = new List<Deserializer>();
                }

                if (!_deserializers[deserializer.PackformatName].Contains(deserializer, DeserializerNameAndVersionComparer.Instance))
                {
                    _deserializers[deserializer.PackformatName].Add(deserializer);
                }
            }

            public static implicit operator DeserializerCollection(DeserializerCollectionBuilder builder)
            {
                return new DeserializerCollection(builder._deserializers);
            }

            private class DeserializerNameAndVersionComparer : IEqualityComparer<Deserializer>
            {
                public static readonly DeserializerNameAndVersionComparer Instance = new DeserializerNameAndVersionComparer();

                public bool Equals(Deserializer x, Deserializer y)
                {
                    return Equals(x.Key, y.Key);
                }

                public int GetHashCode(Deserializer obj)
                {
                    return obj.Key.GetHashCode();
                }
            }
        }
    }
}