using System;
using System.Collections.Generic;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     Class containing information on types that are serializable to/from packformat
    /// </summary>
    internal class SerializationCandidatesCollection
    {
        private readonly Dictionary<Type, SerializerCandidate> _packers;

        private SerializationCandidatesCollection(Dictionary<Type, SerializerCandidate> packers)
        {
            _packers = packers;
        }

        public static SerializationCandidatesCollectionBuilder New
        {
            get { return new SerializationCandidatesCollectionBuilder(); }
        }

        public SerializerCandidate ResolveSerializer(Type type)
        {
            if (!_packers.ContainsKey(type))
            {
                throw Exceptions.SerializerResolutionFailed(type);
            }
            return _packers[type];
        }

        internal class SerializationCandidatesCollectionBuilder
        {
            private readonly Dictionary<Type, SerializerCandidate> _packers =
                new Dictionary<Type, SerializerCandidate>();

            public void AddCandidate(SerializerCandidate candidate)
            {
                if (_packers.ContainsKey(candidate.Type))
                {
                    throw Exceptions.OnlyOneSerializerAllowedForAType(candidate.Type);
                }
                _packers.Add(candidate.Type, candidate);
            }

            public static implicit operator SerializationCandidatesCollection(
                SerializationCandidatesCollectionBuilder builder)
            {
                return new SerializationCandidatesCollection(builder._packers);
            }
        }
    }
}