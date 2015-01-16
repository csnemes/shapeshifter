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
        private readonly SerializableTypeInfo _serializableTypeInfo;
        private readonly Dictionary<string, FieldOrPropertyMemberInfo> _packItemCandidates;

        public DefaultDeserializer(SerializableTypeInfo serializableTypeInfo)
            : base(serializableTypeInfo.PackformatName, serializableTypeInfo.Version)
        {
            _serializableTypeInfo = serializableTypeInfo;
            _packItemCandidates = serializableTypeInfo.Items.ToDictionary(item => item.Name);
        }

        public Type Type
        {
            get { return _serializableTypeInfo.Type; }
        }

        public override Func<ObjectProperties, object> GetDeserializerFunc(SerializerInstanceStore instanceStore)
        {
            return Deserialize;
        }

        private object Deserialize(ObjectProperties packItems)
        {
            object result = FormatterServices.GetUninitializedObject(_serializableTypeInfo.Type);

            //try to find a target for each packItem, if not found skip it
            foreach (var packItem in packItems)
            {
                FieldOrPropertyMemberInfo target;
                if (_packItemCandidates.TryGetValue(packItem.Key, out target))
                {
                    target.SetValueFor(result, ValueConverter.ConvertValueToTargetType(target.Type, packItem.Value));
                }
            }

            return result;
        }
    }
}