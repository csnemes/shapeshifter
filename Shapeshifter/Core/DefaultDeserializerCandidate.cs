using System;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     Default deserializer which uses matching property names to build the instance
    /// </summary>
    internal class DefaultDeserializerCandidate : DeserializerCandidate
    {
        private readonly TypeInfo _typeInfo;
        private Func<ObjectProperties, ValueConverter, object> _unpackerFunc;

        public DefaultDeserializerCandidate(string packformatName, uint version, TypeInfo typeInfo)
            : base(packformatName, version)
        {
            _typeInfo = typeInfo;
        }

        public override Func<ObjectProperties, ValueConverter, object> GetDeserializerFunc()
        {
            if (_unpackerFunc == null)
            {
                var builder = new ReflectionInstanceDeserializer(_typeInfo.Type, _typeInfo.Items);
                _unpackerFunc = builder.Deserialize;
            }
            return _unpackerFunc;
        }
    }
}