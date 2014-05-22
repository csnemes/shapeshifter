using System;

namespace Shapeshifter.Core.Serialization
{
    /// <summary>
    ///     Default serializer which writes all member values with <see cref="DataMember" /> attribute to the
    ///     <see cref="InternalPackformatWriter" />
    /// </summary>
    internal class DefaultSerializer : Serializer
    {
        private readonly TypeInfo _typeInfo;

        public DefaultSerializer(TypeInfo typeInfo)
            : base(typeInfo.Type, typeInfo.Version)
        {
            _typeInfo = typeInfo;
        }

        public override Action<InternalPackformatWriter, object> GetSerializerFunc()
        {
            return SerializeWithReflection;
        }

        private void SerializeWithReflection(InternalPackformatWriter writer, object objToWrite)
        {
            if (objToWrite == null)
            {
                throw new ArgumentNullException("objToWrite");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (_typeInfo.Type != objToWrite.GetType())
            {
                throw Exceptions.InstanceTypeDoesNotMatchSerializerType(_typeInfo.Type, objToWrite.GetType());
            }

            //write type and version
            writer.WriteProperty(Constants.TypeNameKey, _typeInfo.PackformatName);
            writer.WriteProperty(Constants.VersionKey, Convert.ToInt64(_typeInfo.Version));

            //write fields one-by-one
            foreach (var packItemCandidate in _typeInfo.Items)
            {
                writer.WriteProperty(packItemCandidate.Name, packItemCandidate.GetValueFor(objToWrite));
            }
        }
    }
}