using System;
using System.Collections.Generic;
using System.Linq;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     Class containing information on types that are serializable to/from packformat
    /// </summary>
    internal class DeserializationCandidatesCollection
    {
        private readonly Dictionary<string, List<DeserializerCandidate>> _unpackers;

        private DeserializationCandidatesCollection(Dictionary<string, List<DeserializerCandidate>> unpackers)
        {
            _unpackers = unpackers;
        }

        public static DeserializationCandidatesCollectionBuilder New
        {
            get { return new DeserializationCandidatesCollectionBuilder(); }
        }

        public DeserializerCandidate ResolveDeserializer(string typeName, uint version)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }

            if (!_unpackers.ContainsKey(typeName))
            {
                return null;
            }

            List<DeserializerCandidate> unpackers = _unpackers[typeName];

            DeserializerCandidate result = unpackers.FirstOrDefault(item => item.Version == version) ??
                                           unpackers.FirstOrDefault(item => item.Version == 0);

            return result;
        }


        internal class DeserializationCandidatesCollectionBuilder
        {
            private readonly Dictionary<string, List<DeserializerCandidate>> _unpackers =
                new Dictionary<string, List<DeserializerCandidate>>();

            public void AddCandidate(DeserializerCandidate deserializer)
            {
                if (deserializer == null)
                {
                    throw new ArgumentNullException("deserializer");
                }

                if (!_unpackers.ContainsKey(deserializer.PackformatName))
                {
                    _unpackers[deserializer.PackformatName] = new List<DeserializerCandidate>();
                }

                if (
                    !_unpackers[deserializer.PackformatName].Contains(deserializer,
                        DeserializerNameAndVersionComparer.Instance))
                {
                    _unpackers[deserializer.PackformatName].Add(deserializer);
                }
            }

            public static implicit operator DeserializationCandidatesCollection(
                DeserializationCandidatesCollectionBuilder builder)
            {
                return new DeserializationCandidatesCollection(builder._unpackers);
            }

            private class DeserializerNameAndVersionComparer : IEqualityComparer<DeserializerCandidate>
            {
                public static readonly DeserializerNameAndVersionComparer Instance =
                    new DeserializerNameAndVersionComparer();

                public bool Equals(DeserializerCandidate x, DeserializerCandidate y)
                {
                    return Equals(x.Key, y.Key);
                }

                public int GetHashCode(DeserializerCandidate obj)
                {
                    return obj.Key.GetHashCode();
                }
            }
        }
    }
}