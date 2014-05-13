using System;

namespace Shapeshifter.Core
{
    /// <summary>
    ///     Default serializer which writes all member values with <see cref="DataMember" /> attribute to the
    ///     <see cref="IPackformatWriter" />
    /// </summary>
    internal class DefaultSerializerCandidate : SerializerCandidate
    {
        private readonly TypeInfo _typeInfo;
        private Action<IPackformatWriter, object> _packerFunc;

        public DefaultSerializerCandidate(TypeInfo typeInfo)
            : base(typeInfo.Type, typeInfo.Version)
        {
            _typeInfo = typeInfo;
        }

        public override Action<IPackformatWriter, object> GetSerializerFunc()
        {
            if (_packerFunc == null)
            {
                var serializer = new ReflectionInstanceSerializer(_typeInfo);
                _packerFunc = serializer.Serialize;
            }
            return _packerFunc;
        }
    }
}