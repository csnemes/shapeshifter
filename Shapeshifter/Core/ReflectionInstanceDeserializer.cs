using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Shapeshifter.Core
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

        public ReflectionInstanceDeserializer(Type targetType,
            IEnumerable<SerializableTypeMemberInfo> packItemCandidates)
        {
            _targetType = targetType;
            _packItemCandidates = packItemCandidates.ToDictionary(item => item.Name);
        }

        public object Deserialize(ObjectProperties packItems, ConversionHelpers conversionHelpers)
        {
            object result = FormatterServices.GetUninitializedObject(_targetType);

            //try to find a target for each packItem, if not found skip it
            foreach (var packItem in packItems)
            {
                SerializableTypeMemberInfo target;
                if (_packItemCandidates.TryGetValue(packItem.Key, out target))
                {
                    target.SetValueFor(result, conversionHelpers.ConvertValueToTargetType(target.Type, packItem.Value));
                }
            }

            return result;
        }
    }
}