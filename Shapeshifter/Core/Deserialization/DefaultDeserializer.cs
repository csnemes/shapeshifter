using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Shapeshifter.Core.Deserialization
{
    /// <summary>
    ///     Default deserializer which uses matching property names to build the instance
    /// </summary>
    internal class DefaultDeserializer : Deserializer
    {
        private readonly TypeInfo _typeInfo;
        private readonly Dictionary<string, SerializableTypeMemberInfo> _packItemCandidates;

        public DefaultDeserializer(string packformatName, uint version, TypeInfo typeInfo)
            : base(packformatName, version)
        {
            _typeInfo = typeInfo;
            _packItemCandidates = typeInfo.Items.ToDictionary(item => item.Name);
        }

        public override Func<ObjectProperties, object> GetDeserializerFunc()
        {
            return Deserialize;
        }

        private object Deserialize(ObjectProperties packItems)
        {
            object result = FormatterServices.GetUninitializedObject(_typeInfo.Type);

            //try to find a target for each packItem, if not found skip it
            foreach (var packItem in packItems)
            {
                SerializableTypeMemberInfo target;
                if (_packItemCandidates.TryGetValue(packItem.Key, out target))
                {
                    target.SetValueFor(result, ValueConverter.ConvertValueToTargetType(target.Type, packItem.Value));
                }
            }

            return result;
        }
    }
}