using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Shapeshifter.Core.Deserialization
{
    /// <summary>
    ///     This class uses reflection to build up a class instance of type targetType using the data passed as packItems in
    ///     the Deserialize call.
    /// </summary>
    internal class ReflectionInstanceDeserializer
    {
        private readonly Dictionary<string, SerializableTypeMemberInfo> _packItemCandidates;
            //the list of detected members of the type, we'll try to put values

        private readonly Type _targetType; //the type to build

        //into these members

        public ReflectionInstanceDeserializer(Type targetType, IEnumerable<SerializableTypeMemberInfo> packItemCandidates)
        {
            _targetType = targetType;
            _packItemCandidates = packItemCandidates.ToDictionary(item => item.Name);
        }


    }
}